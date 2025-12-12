'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig } from '@/hooks/usePageConfig';
import { useParty } from '@/hooks/useParty';

export default function PartyPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('party');
    const { party, isLoading, error } = useParty();

    // 認証チェック
    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push('/login');
        }
    }, [isAuthenticated, loading, router]);

    // パーティデータのコンソール出力
    useEffect(() => {
        if (party) {
            console.log('Party data:', party);
        }
        if (error) {
            console.error('Party fetch error:', error);
        }
    }, [party, error]);

    if (loading || !isAuthenticated) {
        return null;
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <BottomScreen>
                <div className="flex items-center justify-center h-full">
                    {isLoading ? (
                        <p className="text-xl font-bold">読み込み中...</p>
                    ) : error ? (
                        <p className="text-xl font-bold text-red-600">エラーが発生しました</p>
                    ) : (
                        <p className="text-2xl font-bold text-gray-800">
                            パーティ ({party?.length || 0}匹)
                        </p>
                    )}
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
