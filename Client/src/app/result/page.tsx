"use client";

import { useRouter } from "next/navigation";
import { useEffect } from "react";

import Button from "@/components/atoms/Button";
import BottomScreen from "@/components/organisms/BottomScreen";
import TopScreen from "@/components/organisms/TopScreen";
import DSLayout from "@/components/templates/DSLayout";
import { useAuth } from "@/hooks/useAuth";
import { usePageConfig } from "@/hooks/usePageConfig";
import { useBattleStore } from "@/stores/battleStore";

export default function ResultPage() {
    const { isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig("result");
    const { battleLog, resetBattle } = useBattleStore();

    // テキスト設定
    const t = pageConfig.t;

    // 認証チェック
    useEffect(() => {
        if (!loading && !isAuthenticated) {
            router.push("/login");
        }
    }, [isAuthenticated, loading, router]);

    // ホームに戻る
    const handleBackToHome = () => {
        resetBattle();
        router.push("/");
    };

    if (loading || !isAuthenticated) {
        return null;
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image}>
                <div className="flex h-full flex-col items-center justify-center p-4">
                    <h1 className="text-3xl font-bold text-white drop-shadow-lg">
                        {t?.title || "バトル結果"}
                    </h1>
                </div>
            </TopScreen>

            <BottomScreen darkGrid>
                <div className="flex h-full flex-col p-4">
                    {/* バトルログ表示 */}
                    <div className="mb-4 flex-1 overflow-y-auto rounded-lg bg-black/50 p-3">
                        <h2 className="mb-2 text-lg font-bold text-white">バトルログ</h2>
                        {battleLog.length > 0 ? (
                            <ul className="space-y-1">
                                {battleLog.map((log, index) => (
                                    <li key={index} className="text-sm text-gray-300">
                                        {log}
                                    </li>
                                ))}
                            </ul>
                        ) : (
                            <p className="text-sm text-gray-400">ログがありません</p>
                        )}
                    </div>

                    {/* ホームに戻るボタン */}
                    <Button onClick={handleBackToHome} variant="primary" className="w-full">
                        {t?.backToHome || "ホームに戻る"}
                    </Button>
                </div>
            </BottomScreen>
        </DSLayout>
    );
}
