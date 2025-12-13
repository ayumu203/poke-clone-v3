'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import PartyPokemonCard from '@/components/molecules/PartyPokemonCard';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig } from '@/hooks/usePageConfig';
import { useParty } from '@/hooks/useParty';
import { usePokemonSpeciesBatch } from '@/hooks/usePokemonSpecies';

export default function PartyPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('party');
    const { party, isLoading: isLoadingParty, error } = useParty();

    // パーティに含まれるポケモンの種族情報を取得
    const speciesIds = party?.map(p => p.pokemonSpeciesId) || [];
    const { species: speciesList, isLoading: isLoadingSpecies } = usePokemonSpeciesBatch(speciesIds);

    const isLoading = isLoadingParty || isLoadingSpecies;

    // 認証チェック
    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push('/login');
        }
    }, [isAuthenticated, loading, router]);

    // パーティチェック: パーティが存在しない場合は/starter-pokemonにリダイレクト
    useEffect(() => {
        if (!loading && !isLoadingParty && isAuthenticated && (!party || party.length === 0)) {
            router.push('/starter-pokemon');
        }
    }, [party, isLoadingParty, loading, isAuthenticated, router]);

    if (loading || !isAuthenticated) {
        return null;
    }

    // パーティ未所持の場合は何も表示しない（リダイレクト処理中）
    if (!isLoadingParty && (!party || party.length === 0)) {
        return null;
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <BottomScreen>
                <div className="flex flex-col h-full p-2">
                    {isLoading ? (
                        <div className="flex items-center justify-center h-full">
                            <p className="text-xl font-bold">読み込み中...</p>
                        </div>
                    ) : error ? (
                        <div className="flex items-center justify-center h-full">
                            <p className="text-xl font-bold text-red-600">エラーが発生しました</p>
                        </div>
                    ) : (
                        <div className="grid grid-cols-3 grid-rows-2 gap-2 h-full">
                            {party?.slice(0, 6).map((pokemon, index) => {
                                const species = speciesList[index];
                                if (!species) return null;

                                return (
                                    <PartyPokemonCard
                                        key={pokemon.pokemonId}
                                        pokemon={pokemon}
                                        species={species}
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
