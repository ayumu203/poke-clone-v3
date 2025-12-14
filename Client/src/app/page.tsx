"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";

import MenuButton from "@/components/atoms/MenuButton";
import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { usePageConfig } from "@/hooks/usePageConfig";

export default function Home() {
  const { isAuthenticated, loading } = useAuth();
  const router = useRouter();
  const pageConfig = usePageConfig("home");

  // 認証チェック: 未ログインユーザーを/loginにリダイレクト
  useEffect(() => {
    if (!loading && !isAuthenticated) {
      router.push("/login");
    }
  }, [isAuthenticated, loading, router]);

  // ローディング中は空の画面を表示
  if (loading) {
    return (
      <DSLayout bgmSrc={pageConfig.bgm}>
        <TopScreen backgroundImage={pageConfig.image} />
        <BottomScreen>
          <div className="flex h-full items-center justify-center">
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
    <DSLayout bgmSrc={pageConfig.bgm}>
      <TopScreen backgroundImage={pageConfig.image} />
      <BottomScreen>
        <div className="grid h-full grid-cols-2 content-center items-center justify-items-center gap-8 p-6">
          {pageConfig.menuButtons?.map((button, index) => (
            <MenuButton key={index} imageSrc={button.icon} href={button.href}>
              {button.text}
            </MenuButton>
          ))}
        </div>
      </BottomScreen>
    </DSLayout>
  );
}
