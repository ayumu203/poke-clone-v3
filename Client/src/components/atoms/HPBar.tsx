"use client";

interface HPBarProps {
    /** 現在のHP */
    currentHp: number;
    /** 最大HP */
    maxHp: number;
    /** HP値を表示するか */
    showValue?: boolean;
    /** 追加のクラス名 */
    className?: string;
}

/**
 * HPバーの色を取得
 * 0%, 10%, 25%, 50%, 75%, 100%の閾値で色変化
 */
function getHpBarColor(percentage: number): string {
    if (percentage <= 0) return "#555555"; // 0%: グレー（瀕死）
    if (percentage <= 10) return "#FF0000"; // 10%: 赤
    if (percentage <= 25) return "#FF6600"; // 25%: オレンジ
    if (percentage <= 50) return "#FFCC00"; // 50%: 黄色
    if (percentage <= 75) return "#99CC00"; // 75%: 黄緑
    return "#00CC00"; // 100%: 緑
}

/**
 * HPバーコンポーネント
 * HP残量に応じて色が変化するプログレスバー
 */
export default function HPBar({
    currentHp,
    maxHp,
    showValue = false,
    className = "",
}: HPBarProps) {
    const hpPercentage = maxHp > 0 ? (currentHp / maxHp) * 100 : 0;
    const hpBarColor = getHpBarColor(hpPercentage);

    return (
        <div className={`flex flex-col gap-0.5 ${className}`}>
            {/* HPバー */}
            <div
                className="h-1.5 w-full overflow-hidden rounded-full"
                style={{ backgroundColor: "#444444" }}
            >
                <div
                    className="h-full rounded-full transition-all duration-300"
                    style={{
                        width: `${hpPercentage}%`,
                        backgroundColor: hpBarColor,
                    }}
                />
            </div>

            {/* HP実数値（オプション） */}
            {showValue && (
                <p className="text-lg text-gray-300">
                    HP: {currentHp} / {maxHp}
                </p>
            )}
        </div>
    );
}
