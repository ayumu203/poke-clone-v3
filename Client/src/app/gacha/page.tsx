"use client";

import Image from "next/image";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { useGacha } from "@/hooks/useGacha";
import { usePageConfig } from "@/hooks/usePageConfig";
import { useParty } from "@/hooks/useParty";
import { usePokemonSpecies } from "@/hooks/usePokemonSpecies";

export default function GachaPage() {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("gacha");
  const { party, isLoading: isLoadingParty } = useParty();
  const { pullGachaAsync, isPulling, result, error, isSuccess, reset } = useGacha();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  // 獲得したポケモンの種族情報
  const { species: resultSpecies, isLoading: isLoadingSpecies } = usePokemonSpecies(
    result?.pokemon?.pokemonSpeciesId ?? 0
  );

  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, loading, router]);

  // パーティチェック: パーティが存在しない場合は/starter-pokemonにリダイレクト
  useEffect(() => {
    if (!loading && !isLoadingParty && isAuthenticated && (!party || party.length === 0)) {
      router.push("/starter-pokemon");
    }
  }, [party, isLoadingParty, loading, isAuthenticated, router]);

  if (loading || !isAuthenticated) {
    return null;
  }

  // パーティ未所持の場合は何も表示しない（リダイレクト処理中）
  if (!isLoadingParty && (!party || party.length === 0)) {
    return null;
  }

  const partyCount = party?.length ?? 0;
  const canPull = partyCount < 6;

  const handlePullGacha = async () => {
    setErrorMessage(null);
    try {
      await pullGachaAsync();
    } catch (err) {
      if (err instanceof Error) {
        setErrorMessage(err.message);
      } else {
        setErrorMessage("ガチャの実行に失敗しました");
      }
    }
  };

  const handleReset = () => {
    reset();
    setErrorMessage(null);
  };

  return (
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image} />
      <BottomScreen>
        <div className="flex h-full flex-col p-4">
          {/* ヘッダー情報 */}
          <div className="mb-4 text-center">
            <h1 className="mb-2 text-2xl font-bold">ガチャ</h1>
            <p className="text-sm text-gray-600">パーティ: {partyCount} / 6</p>
            <p className="text-xs text-gray-500">※ガチャ1回: 100円</p>
          </div>

          {/* メインエリア */}
          <div className="flex flex-1 flex-col items-center justify-center">
            {isSuccess && result ? (
              // 結果表示
              <div className="text-center">
                <p className="mb-2 text-lg font-bold text-green-600">ポケモンを獲得しました！</p>
                {isLoadingSpecies ? (
                  <p>読み込み中...</p>
                ) : resultSpecies ? (
                  <div className="rounded border-2 border-gray-800 bg-white p-4">
                    <Image
                      src={resultSpecies.frontImage}
                      alt={resultSpecies.name}
                      width={96}
                      height={96}
                      className="mx-auto"
                      unoptimized
                    />
                    <p className="text-xl font-bold">{resultSpecies.name}</p>
                    <p className="text-sm text-gray-600">Lv.{result.pokemon.level}</p>
                  </div>
                ) : (
                  <p>種族情報を取得できませんでした</p>
                )}
                <button
                  onClick={handleReset}
                  className="mt-4 border-2 border-blue-700 bg-blue-500 px-6 py-2 font-bold text-white hover:bg-blue-600"
                >
                  もう一度引く
                </button>
              </div>
            ) : (
              // ガチャボタン
              <div className="text-center">
                {!canPull ? (
                  <p className="mb-4 font-bold text-red-600">
                    パーティが満員です！
                    <br />
                    ポケモンを逃がしてから再度お試しください。
                  </p>
                ) : null}

                {errorMessage && <p className="mb-4 font-bold text-red-600">{errorMessage}</p>}

                {error && <p className="mb-4 font-bold text-red-600">エラー: {error.message}</p>}

                <button
                  onClick={handlePullGacha}
                  disabled={isPulling || !canPull}
                  className="border-4 border-yellow-600 bg-yellow-400 px-8 py-4 text-xl font-bold text-black hover:bg-yellow-500 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {isPulling ? "ガチャ中..." : "ガチャを引く！"}
                </button>
              </div>
            )}
          </div>

          {/* フッター */}
          <div className="mt-4 text-center">
            <button
              onClick={() => router.push("/")}
              className="border-2 border-gray-500 bg-gray-300 px-4 py-2 font-bold text-gray-800 hover:bg-gray-400"
            >
              ホームに戻る
            </button>
          </div>
        </div>
      </BottomScreen>
    </DSLayout>
  );
}
