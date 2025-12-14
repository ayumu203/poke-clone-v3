"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";

import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { usePageConfig } from "@/hooks/usePageConfig";
import { useParty } from "@/hooks/useParty";

export default function OnlineBattlePage() {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("online-battle");
  const { party, isLoading } = useParty();

  // 認証チェック
  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, loading, router]);

  // パーティチェック: パーティが存在しない場合は/starter-pokemonにリダイレクト
  useEffect(() => {
    if (!loading && !isLoading && isAuthenticated && (!party || party.length === 0)) {
      router.push("/starter-pokemon");
    }
  }, [party, isLoading, loading, isAuthenticated, router]);

  if (loading || !isAuthenticated) {
    return null;
  }

  // パーティ未所持の場合は何も表示しない（リダイレクト処理中）
  if (!isLoading && (!party || party.length === 0)) {
    return null;
  }

  return (
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image} />
      <BottomScreen>
        <div className="flex h-full items-center justify-center">
          <p className="text-2xl font-bold text-gray-800">オンライン対戦</p>
        </div>
      </BottomScreen>
    </DSLayout>
  );
}
