"use client";

import { useRouter } from "next/navigation";
import { useCallback, useEffect, useRef, useState } from "react";

import BattleCommandPanel from "@/components/molecules/BattleCommandPanel";
import BattlePokemonPanel from "@/components/molecules/BattlePokemonPanel";
import MoveCommandPanel from "@/components/molecules/MoveCommandPanel";
import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { useBattle } from "@/hooks/useBattle";
import { usePageConfig } from "@/hooks/usePageConfig";
import { useParty } from "@/hooks/useParty";
import { usePokemonSpeciesBatch } from "@/hooks/usePokemonSpecies";
import { useSignalR } from "@/hooks/useSignalR";
import { useBattleStore } from "@/stores/battleStore";
import { ActionType } from "@/types/action";
import type { BattleState, EndResult, ProcessResult } from "@/types/battle";
import type { PokemonSpecies } from "@/types/pokemonSpecies";
import { generateBattleLogs } from "@/utils/battleLogUtils";

type CommandMode = "main" | "attack";

export default function CpuBattlePage() {
  const { isAuthenticated, loading, user } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("cpu-battle");
  const { party, isLoading: isLoadingParty } = useParty();
  const { createCpuBattle, isCreating } = useBattle();
  const { battleState, setBattleState, addLog, setWaitStatus, waitStatus } =
    useBattleStore();

  // バトルID管理
  const [battleId, setBattleId] = useState<string | null>(null);
  // コマンドモード
  const [commandMode, setCommandMode] = useState<CommandMode>("main");
  // バトル終了フラグ
  const [battleEnded, setBattleEnded] = useState(false);
  // 結果メッセージ
  const [resultMessage, setResultMessage] = useState<string>("");
  // IDトークン（SignalR接続用）
  const [idToken, setIdToken] = useState<string>("");

  // テキスト設定
  const t = pageConfig.t;

  // パーティのポケモン種族情報を取得
  const partySpeciesIds = party?.map((p) => p.pokemonSpeciesId) || [];
  // partySpeciesは技選択画面で将来使用予定
  usePokemonSpeciesBatch(partySpeciesIds);

  // バトル状態のポケモン種族情報を取得
  const battleSpeciesIds =
    battleState?.player1.party.map((p) => p.pokemonSpeciesId).concat(
      battleState?.player2.party.map((p) => p.pokemonSpeciesId) || []
    ) || [];
  const { species: battleSpecies } = usePokemonSpeciesBatch(battleSpeciesIds);

  // refで最新の状態を保持（useCallback内で参照するため）
  const battleStateRef = useRef<BattleState | null>(null);
  const battleSpeciesRef = useRef<(PokemonSpecies | undefined)[]>([]);
  const partyRef = useRef(party);

  useEffect(() => {
    battleStateRef.current = battleState;
  }, [battleState]);

  useEffect(() => {
    battleSpeciesRef.current = battleSpecies;
  }, [battleSpecies]);

  useEffect(() => {
    partyRef.current = party;
  }, [party]);

  // SignalRコールバック
  const handleTurnResult = useCallback(
    (result: ProcessResult) => {
      // わかりやすいログを生成
      if (battleStateRef.current) {
        const logs = generateBattleLogs(result, {
          battleState: battleStateRef.current,
          species: battleSpeciesRef.current,
          party: partyRef.current || [],
        });

        // コンソールとストアに出力
        logs.forEach((log) => {
          console.warn(`[BATTLE] ${log}`);
          addLog(log);
        });
      } else {
        // フォールバック（battleStateがない場合）
        console.warn("=== ターン結果 ===", result);
      }

      // 待機状態を解除
      setWaitStatus("waiting-action");
      setCommandMode("main");
    },
    [addLog, setWaitStatus]
  );

  const handleBattleStarted = useCallback(
    (state: BattleState) => {
      console.log("=== バトル開始 ===", state);
      setBattleState(state);
      setWaitStatus("waiting-action");
      addLog("バトル開始!");
    },
    [setBattleState, setWaitStatus, addLog]
  );

  const handleBattleEnded = useCallback(
    (endResult: EndResult) => {
      console.log("=== バトル終了 ===", endResult);
      setBattleEnded(true);

      // 勝敗判定
      const isWin = endResult.winnerId === user?.uid;
      setResultMessage(isWin ? (t?.messages?.win || "勝利!") : (t?.messages?.lose || "敗北..."));
      addLog(isWin ? "勝利!" : "敗北...");

      // 3秒後にresultページへ遷移
      setTimeout(() => {
        router.push("/result");
      }, 3000);
    },
    [user?.uid, t, addLog, router]
  );

  // IDトークン取得
  useEffect(() => {
    const fetchToken = async () => {
      if (user) {
        try {
          const token = await user.getIdToken();
          setIdToken(token);
        } catch (error) {
          console.error("トークン取得エラー:", error);
        }
      }
    };
    fetchToken();
  }, [user]);

  // SignalR接続（バトルIDがある場合のみ）
  const { submitAction, isConnected } = useSignalR(
    battleId || "",
    idToken,
    handleTurnResult,
    handleBattleStarted,
    handleBattleEnded
  );

  // 認証チェック
  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, loading, router]);

  // パーティチェック
  useEffect(() => {
    if (!loading && !isLoadingParty && isAuthenticated && (!party || party.length === 0)) {
      router.push("/starter-pokemon");
    }
  }, [party, isLoadingParty, loading, isAuthenticated, router]);

  // クリーンアップはresultページで実行する（ログ保持のため）

  // バトル開始処理
  const handleStartBattle = async () => {
    try {
      createCpuBattle(undefined, {
        onSuccess: (data) => {
          if (data?.battleId) {
            setBattleId(data.battleId);
            console.log("バトル作成成功:", data.battleId);
          }
        },
        onError: (error) => {
          console.error("バトル作成エラー:", error);
        },
      });
    } catch (error) {
      console.error("バトル開始エラー:", error);
    }
  };

  // コマンド選択ハンドラ
  const handleAttackCommand = () => {
    setCommandMode("attack");
  };

  const handleCatchCommand = async () => {
    if (!user?.uid) return;
    setWaitStatus("waiting-opponent");
    try {
      await submitAction({
        playerId: user.uid,
        actionType: ActionType.Catch,
        value: 0,
      });
    } catch (error) {
      console.error("アクション送信エラー:", error);
      setWaitStatus("waiting-action");
    }
  };

  const handleEscapeCommand = async () => {
    if (!user?.uid) return;
    // 逃走の場合はActionType.Escapeを使う（existingの型定義に追加が必要かも）
    // 現状はCatchを使って代用、または別途対応
    setResultMessage(t?.messages?.escaped || "うまく逃げ切れた！");
    setBattleEnded(true);
    setTimeout(() => {
      router.push("/result");
    }, 2000);
  };

  const handleMoveSelect = async (moveId: number) => {
    if (!user?.uid) return;
    setWaitStatus("waiting-opponent");
    try {
      await submitAction({
        playerId: user.uid,
        actionType: ActionType.Attack,
        value: moveId,
      });
    } catch (error) {
      console.error("アクション送信エラー:", error);
      setWaitStatus("waiting-action");
    }
  };

  const handleBackToMain = () => {
    setCommandMode("main");
  };

  // ローディング状態
  if (loading || !isAuthenticated) {
    return null;
  }

  if (!isLoadingParty && (!party || party.length === 0)) {
    return null;
  }

  // 自分のアクティブポケモンの技を取得
  const playerMoves = party?.[0]?.moves || [];

  // バトル開始前の画面
  if (!battleId) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen darkGrid>
          <div className="flex h-full flex-col items-center justify-center gap-4 p-4">
            <h1 className="text-2xl font-bold text-white">CPU対戦</h1>
            <button
              onClick={handleStartBattle}
              disabled={isCreating}
              className="rounded-lg bg-blue-600 px-8 py-4 text-xl font-bold text-white transition-colors hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {isCreating ? (t?.connecting || "接続中...") : (t?.startBattle || "バトル開始")}
            </button>
          </div>
        </BottomScreen>
      </DSLayout>
    );
  }

  // 接続待ち
  if (!isConnected || !battleState) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen darkGrid>
          <div className="flex h-full items-center justify-center">
            <p className="text-xl font-bold text-white">{t?.connecting || "接続中..."}</p>
          </div>
        </BottomScreen>
      </DSLayout>
    );
  }

  // バトル終了画面
  if (battleEnded) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen darkGrid>
          <div className="flex h-full flex-col items-center justify-center gap-4">
            <p className="text-3xl font-bold text-white">{resultMessage}</p>
            <p className="text-lg text-gray-300">結果画面へ移動します...</p>
          </div>
        </BottomScreen>
      </DSLayout>
    );
  }

  // プレイヤーと相手を特定
  const isPlayer1 = battleState.player1.playerId === user?.uid;
  const playerState = isPlayer1 ? battleState.player1 : battleState.player2;
  const opponentState = isPlayer1 ? battleState.player2 : battleState.player1;

  // アクティブポケモン
  const playerActivePokemon = playerState.party[playerState.activePokemonIndex];
  const opponentActivePokemon = opponentState.party[opponentState.activePokemonIndex];

  // 種族情報（バトル中の情報から取得）
  const playerSpeciesIndex = battleState.player1.party
    .concat(battleState.player2.party)
    .findIndex((p) => p.pokemonId === playerActivePokemon?.pokemonId);
  const opponentSpeciesIndex = battleState.player1.party
    .concat(battleState.player2.party)
    .findIndex((p) => p.pokemonId === opponentActivePokemon?.pokemonId);

  const playerPokemonSpecies = battleSpecies[playerSpeciesIndex];
  const opponentPokemonSpecies = battleSpecies[opponentSpeciesIndex];

  const isWaiting = waitStatus === "waiting-opponent" || waitStatus === "processing";

  return (
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image}>
        <div className="flex h-full flex-col justify-between p-2 md:p-4">
          {/* 相手ポケモン（上部） */}
          {opponentActivePokemon && opponentPokemonSpecies && (
            <BattlePokemonPanel
              pokemonState={opponentActivePokemon}
              species={opponentPokemonSpecies}
              isPlayer={false}
              className="self-end"
            />
          )}

          {/* 自分ポケモン（下部） */}
          {playerActivePokemon && playerPokemonSpecies && (
            <BattlePokemonPanel
              pokemonState={playerActivePokemon}
              species={playerPokemonSpecies}
              isPlayer={true}
              className="self-start"
            />
          )}
        </div>
      </TopScreen>

      <BottomScreen darkGrid hideFooter>
        {/* 待機中メッセージ */}
        {isWaiting ? (
          <div className="flex h-full items-center justify-center">
            <p className="text-xl font-bold text-white">
              {t?.waitingOpponent || "相手の行動を待っています..."}
            </p>
          </div>
        ) : commandMode === "main" ? (
          /* メインコマンド */
          <BattleCommandPanel
            onAttack={handleAttackCommand}
            onCatch={handleCatchCommand}
            onEscape={handleEscapeCommand}
            texts={t?.commands}
            disabled={isWaiting}
          />
        ) : (
          /* 技選択 */
          <MoveCommandPanel
            moves={playerMoves}
            onMoveSelect={handleMoveSelect}
            onBack={handleBackToMain}
            backText={t?.commands?.back}
            disabled={isWaiting}
          />
        )}
      </BottomScreen>
    </DSLayout>
  );
}
