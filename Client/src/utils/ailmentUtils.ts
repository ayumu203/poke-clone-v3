"use client";

import type { Ailment } from "@/types/battle";

/**
 * 状態異常を日本語ラベルに変換
 */
export function getAilmentLabel(ailment: Ailment): string {
    const labels: Record<Ailment, string> = {
        None: "",
        Paralysis: "まひ",
        Sleep: "ねむり",
        Freeze: "こおり",
        Burn: "やけど",
        Poison: "どく",
        DeadlyPoisoned: "もうどく",
    };
    return labels[ailment] || "";
}

/**
 * 状態異常に対応する背景色を取得
 */
export function getAilmentColor(ailment: Ailment): string {
    const colors: Record<Ailment, string> = {
        None: "transparent",
        Paralysis: "#F8D030", // 黄色
        Sleep: "#A8A878", // グレー
        Freeze: "#98D8D8", // 水色
        Burn: "#F08030", // オレンジ
        Poison: "#A040A0", // 紫
        DeadlyPoisoned: "#700070", // 濃い紫
    };
    return colors[ailment] || "transparent";
}
