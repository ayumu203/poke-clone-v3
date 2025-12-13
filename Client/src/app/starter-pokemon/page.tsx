'use client';

import { useQueryClient } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { M_PLUS_Rounded_1c } from 'next/font/google';

import BottomScreen from '@/components/organisms/BottomScreen';
import MessageScreen from '@/components/organisms/MessageScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig, useCommonTexts } from '@/hooks/usePageConfig';
import { useParty } from '@/hooks/useParty';
import { useStarter } from '@/hooks/useStarter';
import StarterPokemonCard from '@/components/molecules/StarterPokemonCard';
import pokemonDescriptions from '@/data/pokemon/pokemonDescriptions.json';

const mPlusRounded = M_PLUS_Rounded_1c({
    weight: '800',
    subsets: ['latin'],
    display: 'swap',
});

export default function StarterPokemonPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('starter-pokemon');
    const commonTexts = useCommonTexts();
    const { options, isLoading: isLoadingOptions, selectStarter, isSelecting } = useStarter();
    const { party, isLoading: isLoadingParty } = useParty();
    const [selectedPokemon, setSelectedPokemon] = useState<number | null>(null);
    const [isCompleted, setIsCompleted] = useState(false);
    const queryClient = useQueryClient();

    const { t } = pageConfig;

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
                // パーティキャッシュを無効化
                queryClient.invalidateQueries({ queryKey: ['party'] });
                setIsCompleted(true);
            },
        });
    };

    // ローディング中は空の画面を表示
    if (loading || isLoadingOptions || isLoadingParty) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen backgroundImage="/Images/gift_image.png">
                    <MessageScreen message={commonTexts.loading} messageClassName="text-xl font-bold" />
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
                <BottomScreen backgroundImage="/Images/gift_image.png">
                    <MessageScreen
                        message={t.alreadyHasParty}
                        messageClassName="text-4xl mt-10 text-white font-bold text-center px-8"
                    />
                </BottomScreen>
            </DSLayout>
        );
    }

    // 選択完了後の画面
    if (isCompleted) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen backgroundImage="/Images/gift_image.png">
                    <MessageScreen message={t.selectionComplete} />
                </BottomScreen>
            </DSLayout>
        );
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <BottomScreen backgroundImage="/Images/gift_image1.png">
                <div className="h-full p-6 flex flex-col items-center justify-start overflow-y-auto">
                    <h1
                        className={`text-3xl font-bold mb-10 text-center ${mPlusRounded.className}`}
                        style={{
                            color: 'white',
                            WebkitTextStroke: '1.5px #E0BFB8'
                        }}
                    >
                        {t.title}
                    </h1>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 w-full max-w-6xl">
                        {options?.map((pokemon) => (
                            <StarterPokemonCard
                                key={pokemon.pokemonSpeciesId}
                                pokemon={pokemon}
                                description={
                                    pokemonDescriptions[pokemon.pokemonSpeciesId.toString() as keyof typeof pokemonDescriptions]
                                }
                                onClick={() => handleSelectStarter(pokemon.pokemonSpeciesId)}
                                disabled={isSelecting}
                            />
                        ))}
                    </div>
                    {isSelecting && (
                        <p className="mt-4 text-lg font-bold">{t.selecting}</p>
                    )}
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
