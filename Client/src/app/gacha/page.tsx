'use client';

import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { useGacha } from '@/hooks/useGacha';
import { usePageConfig } from '@/hooks/usePageConfig';
import { useParty } from '@/hooks/useParty';
import { usePokemonSpecies } from '@/hooks/usePokemonSpecies';

export default function GachaPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('gacha');
    const { party, isLoading: isLoadingParty } = useParty();
    const { pullGachaAsync, isPulling, result, error, isSuccess, reset } = useGacha();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    // 獲得したポケモンの種族情報
    const { species: resultSpecies, isLoading: isLoadingSpecies } = usePokemonSpecies(
        result?.pokemon?.pokemonSpeciesId ?? 0
    );

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
                setErrorMessage('ガチャの実行に失敗しました');
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
                <div className="flex flex-col h-full p-4">
                    {/* ヘッダー情報 */}
                    <div className="text-center mb-4">
                        <h1 className="text-2xl font-bold mb-2">ガチャ</h1>
                        <p className="text-sm text-gray-600">
                            パーティ: {partyCount} / 6
                        </p>
                        <p className="text-xs text-gray-500">
                            ※ガチャ1回: 100円
                        </p>
                    </div>

                    {/* メインエリア */}
                    <div className="flex-1 flex flex-col items-center justify-center">
                        {isSuccess && result ? (
                            // 結果表示
                            <div className="text-center">
                                <p className="text-lg font-bold text-green-600 mb-2">
                                    ポケモンを獲得しました！
                                </p>
                                {isLoadingSpecies ? (
                                    <p>読み込み中...</p>
                                ) : resultSpecies ? (
                                    <div className="bg-white border-2 border-gray-800 p-4 rounded">
                                        <img
                                            src={resultSpecies.frontImage}
                                            alt={resultSpecies.name}
                                            className="w-24 h-24 mx-auto"
                                        />
                                        <p className="font-bold text-xl">{resultSpecies.name}</p>
                                        <p className="text-sm text-gray-600">
                                            Lv.{result.pokemon.level}
                                        </p>
                                    </div>
                                ) : (
                                    <p>種族情報を取得できませんでした</p>
                                )}
                                <button
                                    onClick={handleReset}
                                    className="mt-4 px-6 py-2 bg-blue-500 text-white font-bold border-2 border-blue-700 hover:bg-blue-600"
                                >
                                    もう一度引く
                                </button>
                            </div>
                        ) : (
                            // ガチャボタン
                            <div className="text-center">
                                {!canPull ? (
                                    <p className="text-red-600 font-bold mb-4">
                                        パーティが満員です！<br />
                                        ポケモンを逃がしてから再度お試しください。
                                    </p>
                                ) : null}

                                {errorMessage && (
                                    <p className="text-red-600 font-bold mb-4">
                                        {errorMessage}
                                    </p>
                                )}

                                {error && (
                                    <p className="text-red-600 font-bold mb-4">
                                        エラー: {error.message}
                                    </p>
                                )}

                                <button
                                    onClick={handlePullGacha}
                                    disabled={isPulling || !canPull}
                                    className="px-8 py-4 bg-yellow-400 text-black font-bold text-xl border-4 border-yellow-600 hover:bg-yellow-500 disabled:opacity-50 disabled:cursor-not-allowed"
                                >
                                    {isPulling ? 'ガチャ中...' : 'ガチャを引く！'}
                                </button>
                            </div>
                        )}
                    </div>

                    {/* フッター */}
                    <div className="text-center mt-4">
                        <button
                            onClick={() => router.push('/')}
                            className="px-4 py-2 bg-gray-300 text-gray-800 font-bold border-2 border-gray-500 hover:bg-gray-400"
                        >
                            ホームに戻る
                        </button>
                    </div>
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
