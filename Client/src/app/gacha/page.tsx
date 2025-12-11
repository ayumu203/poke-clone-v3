'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig } from '@/hooks/usePageConfig';

export default function GachaPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('gacha');

    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push('/login');
        }
    }, [isAuthenticated, loading, router]);

    if (loading || !isAuthenticated) {
        return null;
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <BottomScreen>
                <div className="flex items-center justify-center h-full">
                    <p className="text-2xl font-bold text-gray-800">ガチャ</p>
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
