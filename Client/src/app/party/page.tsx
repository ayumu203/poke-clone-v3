"use client";

import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";

import ConfirmDialog from "@/components/molecules/ConfirmDialog";
import PartyPokemonCard from "@/components/molecules/PartyPokemonCard";
import PokemonDetailBottom from "@/components/molecules/PokemonDetailBottom";
import PokemonDetailTop from "@/components/molecules/PokemonDetailTop";
import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { usePageConfig } from "@/hooks/usePageConfig";
import { useParty } from "@/hooks/useParty";
import { usePokemonSpeciesBatch } from "@/hooks/usePokemonSpecies";

export default function PartyPage() {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("party");
  const { party, isLoading: isLoadingParty, error, releasePokemon, isReleasing } = useParty();

  // ポケモン選択状態
  const [selectedPokemonId, setSelectedPokemonId] = useState<string | null>(null);
  // 確認ダイアログ表示状態
  const [showReleaseConfirm, setShowReleaseConfirm] = useState(false);

  // パーティに含まれるポケモンの種族情報を取得
  const speciesIds = party?.map((p) => p.pokemonSpeciesId) || [];
  const { species: speciesList, isLoading: isLoadingSpecies } = usePokemonSpeciesBatch(speciesIds);

  const isLoading = isLoadingParty || isLoadingSpecies;

  // 選択中のポケモンを取得
  const selectedPokemon = party?.find((p) => p.pokemonId === selectedPokemonId);
  const selectedIndex = party?.findIndex((p) => p.pokemonId === selectedPokemonId) ?? -1;
  const selectedSpecies = selectedIndex >= 0 ? speciesList[selectedIndex] : undefined;

  // テキスト設定
  const { t } = pageConfig;

  // 認証チェック
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



  const handlePokemonClick = (pokemonId: string) => {
    setSelectedPokemonId(pokemonId);
  };

  const handleBack = () => {
    setSelectedPokemonId(null);
  };

  const handleReleaseClick = () => {
    setShowReleaseConfirm(true);
  };

  const handleReleaseConfirm = () => {
    if (selectedPokemonId) {
      releasePokemon(selectedPokemonId);
      setShowReleaseConfirm(false);
      // ポケモンを逃がした後、選択を解除
      setSelectedPokemonId(null);
    }
  };

  const handleReleaseCancel = () => {
    setShowReleaseConfirm(false);
  };

  if (loading || !isAuthenticated) {
    return null;
  }

  // パーティ未所持の場合は何も表示しない（リダイレクト処理中）
  if (!isLoadingParty && (!party || party.length === 0)) {
    return null;
  }

  // ポケモン詳細表示モード
  if (selectedPokemon && selectedSpecies) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image}>
          <PokemonDetailTop
            pokemon={selectedPokemon}
            species={selectedSpecies}
            texts={
              t?.stats || {
                attack: "こうげき",
                defense: "ぼうぎょ",
                spAttack: "とくこう",
                spDefense: "とくぼう",
                speed: "すばやさ",
              }
            }
          />
        </TopScreen>
        <BottomScreen darkGrid hideFooter>
          <PokemonDetailBottom
            pokemon={selectedPokemon}
            texts={{
              release: t?.release || "逃がす",
              back: t?.back || "戻る",
            }}
            onBack={handleBack}
            onRelease={handleReleaseClick}
            isReleasing={isReleasing}
          />
        </BottomScreen>

        {/* 確認ダイアログ */}
        <ConfirmDialog
          isOpen={showReleaseConfirm}
          message={t?.releaseConfirm || "本当にこのポケモンを逃がしますか？"}
          confirmText={t?.release || "逃がす"}
          cancelText={t?.releaseCancel || "キャンセル"}
          onConfirm={handleReleaseConfirm}
          onCancel={handleReleaseCancel}
          isLoading={isReleasing}
          variant="danger"
        />
      </DSLayout>
    );
  }

  // パーティ一覧表示モード
  return (
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image} />
      <BottomScreen darkGrid>
        <div className="flex h-full flex-col p-2">
          {isLoading ? (
            <div className="flex h-full items-center justify-center">
              <p className="text-xl font-bold text-white">{t?.loading || "読み込み中..."}</p>
            </div>
          ) : error ? (
            <div className="flex h-full items-center justify-center">
              <p className="text-xl font-bold text-red-400">{t?.error || "エラーが発生しました"}</p>
            </div>
          ) : (
            <div className="grid h-full grid-cols-2 grid-rows-3 gap-2 overflow-y-auto md:grid-cols-3 md:grid-rows-2">
              {party?.slice(0, 6).map((pokemon, index) => {
                const species = speciesList[index];
                if (!species) return null;

                return (
                  <PartyPokemonCard
                    key={pokemon.pokemonId}
                    pokemon={pokemon}
                    species={species}
                    onClick={() => handlePokemonClick(pokemon.pokemonId)}
                  />
                );
              })}
            </div>
          )}
        </div>
      </BottomScreen>
    </DSLayout>
  );
}
