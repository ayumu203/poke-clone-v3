'use client';

import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';

export default function Home() {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();

  // 認証チェック: 未ログインユーザーを/loginにリダイレクト
  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, loading, router]);

  // ローディング中は空の画面を表示
  if (loading) {
    return (
      <DSLayout>
        <TopScreen backgroundImage="/Images/art_003.png" />
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

  return (
    <DSLayout bgmSrc="/audio/02_masara_town.mp3">
      <TopScreen backgroundImage="/Images/art_003.png" />
      <BottomScreen />
    </DSLayout>
  );
}
