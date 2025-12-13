'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig } from '@/hooks/usePageConfig';
import { useParty } from '@/hooks/useParty';
import { useStarter } from '@/hooks/useStarter';

export default function StarterPokemonPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('home');
    const { options, isLoading: isLoadingOptions, selectStarter, isSelecting } = useStarter();
    const { party, isLoading: isLoadingParty } = useParty();
    const [selectedPokemon, setSelectedPokemon] = useState<number | null>(null);
    const [isCompleted, setIsCompleted] = useState(false);

    // 認証チェック: 未ログインユーザーを/loginにリダイレクト
    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push('/login');
        }
    }, [isAuthenticated, loading, router]);

    // 選択完了後、パーティ情報をコンソールに表示
    useEffect(() => {
        if (isCompleted && party && party.length > 0) {
            console.log('パーティ情報:', party);
        }
    }, [isCompleted, party]);

    // スターター選択処理
    const handleSelectStarter = (pokemonSpeciesId: number) => {
        setSelectedPokemon(pokemonSpeciesId);
        selectStarter(pokemonSpeciesId, {
            onSuccess: () => {
                setIsCompleted(true);
            },
        });
    };

    // ローディング中は空の画面を表示
    if (loading || isLoadingOptions || isLoadingParty) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen>
                    <div className="flex items-center justify-center h-full">
                        <p className="text-xl font-bold">読み込み中...</p>
                    </div>
                </BottomScreen>
            </DSLayout>
        );
    }

    // 未認証の場合は何も表示しない（リダイレクト処理中）
    if (!isAuthenticated) {
        return null;
    }

    // 既にパーティを持つユーザーの場合
    if (party && party.length > 0) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen>
                    <div className="flex items-center justify-center h-full">
                        <p className="text-2xl font-bold text-center px-8">
                            君にはもう仲間がいます
                        </p>
                    </div>
                </BottomScreen>
            </DSLayout>
        );
    }

    // 選択完了後の画面
    if (isCompleted) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen>
                    <div className="flex items-center justify-center h-full">
                        <p className="text-2xl font-bold text-center px-8">
                            選択完了
                        </p>
                    </div>
                </BottomScreen>
            </DSLayout>
        );
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <BottomScreen>
                <div className="h-full p-6 flex flex-col items-center justify-center">
                    <h1 className="text-2xl font-bold mb-6 text-center">
                        スターターポケモンを選んでください
                    </h1>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4 w-full max-w-4xl">
                        {options?.map((pokemon) => (
                            <button
                                key={pokemon.pokemonSpeciesId}
                                onClick={() => handleSelectStarter(pokemon.pokemonSpeciesId)}
                                disabled={isSelecting}
                                className="bg-white border-4 border-gray-800 rounded-lg p-4 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                            >
                                <div className="flex flex-col items-center gap-2">
                                    {pokemon.frontImage && (
                                        <img
                                            src={pokemon.frontImage}
                                            alt={pokemon.name}
                                            className="w-24 h-24 object-contain"
                                        />
                                    )}
                                    <p className="text-lg font-bold">{pokemon.name}</p>
                                    <div className="text-sm text-gray-600">
                                        <p>タイプ: {pokemon.type1}{pokemon.type2 ? ` / ${pokemon.type2}` : ''}</p>
                                    </div>
                                </div>
                            </button>
                        ))}
                    </div>
                    {isSelecting && (
                        <p className="mt-4 text-lg font-bold">選択中...</p>
                    )}
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
