"use client";

import { getTypeColors } from "@/utils/typeColors";
import { convertTypeEnumToString } from "@/utils/typeEnum";

interface TypeBadgeProps {
    /** タイプ（Enum値または文字列） */
    type: number | string;
    /** サイズ: "sm" | "md" | "lg" */
    size?: "sm" | "md" | "lg";
    /** 追加のクラス名 */
    className?: string;
}

/**
 * ポケモンタイプバッジコンポーネント
 * タイプに応じた色でバッジを表示
 */
export default function TypeBadge({ type, size = "md", className = "" }: TypeBadgeProps) {
    // タイプが数値の場合は文字列に変換
    const typeString = typeof type === "number" ? convertTypeEnumToString(type) : type;
    const typeColors = getTypeColors(typeString);

    // サイズに応じたスタイル
    const sizeStyles = {
        sm: "px-1 py-0.5 text-xs",
        md: "px-2 py-0.5 text-sm",
        lg: "px-4 py-1 text-base",
    };

    return (
        <span
            className={`rounded font-bold ${sizeStyles[size]} ${className}`}
            style={{
                backgroundColor: typeColors.border,
                color: "#ffffff",
            }}
        >
            {typeString}
        </span>
    );
}
