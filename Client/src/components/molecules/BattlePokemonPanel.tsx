"use client";

import Image from "next/image";

import HPBar from "@/components/atoms/HPBar";
import type { Ailment, PokemonState } from "@/types/battle";
import type { PokemonSpecies } from "@/types/pokemonSpecies";
import { getAilmentColor, getAilmentLabel } from "@/utils/ailmentUtils";

interface BattlePokemonPanelProps {
    /** ポケモンの状態 */
    pokemonState: PokemonState;
    /** ポケモンの種族情報 */
    species: PokemonSpecies;
    /** 自分のポケモンかどうか（falseの場合は相手） */
    isPlayer: boolean;
    /** 追加のクラス名 */
    className?: string;
}

/**
 * バトル中のポケモン情報表示パネル
 * - ポケモン画像（自分: backImage, 相手: frontImage）
 * - 名前・レベル
 * - HPバー
 * - 状態異常バッジ
 */
export default function BattlePokemonPanel({
    pokemonState,
    species,
    isPlayer,
    className = "",
}: BattlePokemonPanelProps) {
    const imageUrl = isPlayer ? species.backImage : species.frontImage;
    const ailmentLabel = getAilmentLabel(pokemonState.ailment);
    const ailmentColor = getAilmentColor(pokemonState.ailment);

    return (
        <div
            className={`flex items-center gap-3 rounded-lg p-2 ${className}`}
            style={{ backgroundColor: "rgba(0, 0, 0, 0.6)" }}
        >
            {/* ポケモン画像 */}
            <div className="shrink-0">
                <Image
                    src={imageUrl}
                    alt={species.name}
                    width={80}
                    height={80}
                    className="h-16 w-16 object-contain md:h-20 md:w-20"
                    unoptimized
                />
            </div>

            {/* 情報エリア */}
            <div className="flex flex-1 flex-col gap-1">
                {/* 名前とレベル */}
                <div className="flex items-center justify-between">
                    <span className="text-base font-bold text-white md:text-lg">{species.name}</span>
                    <span className="text-sm text-gray-300">Lv.{pokemonState.maxHp > 0 ? Math.floor((pokemonState.maxHp - 10) / 2) : 1}</span>
                </div>

                {/* HPバー */}
                <HPBar
                    currentHp={pokemonState.currentHp}
                    maxHp={pokemonState.maxHp}
                    showValue
                    className="text-sm"
                />

                {/* 状態異常 */}
                {ailmentLabel && (
                    <span
                        className="w-fit rounded px-2 py-0.5 text-xs font-bold text-white"
                        style={{ backgroundColor: ailmentColor }}
                    >
                        {ailmentLabel}
                    </span>
                )}
            </div>
        </div>
    );
}
