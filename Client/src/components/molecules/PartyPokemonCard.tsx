'use client';

import Image from 'next/image';

import type { Pokemon } from '@/types/pokemon';
import type { PokemonSpecies } from '@/types/pokemonSpecies';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';
import { calcHp } from '@/utils/statCalculator';

interface PartyPokemonCardProps {
    pokemon: Pokemon;
    species: PokemonSpecies;
}

/**
 * HPバーの色を取得
 * 0%, 10%, 25%, 50%, 75%, 100%の閾値で色変化
 */
function getHpBarColor(percentage: number): string {
    if (percentage <= 0) return '#555555';      // 0%: グレー（瀕死）
    if (percentage <= 10) return '#FF0000';     // 10%: 赤
    if (percentage <= 25) return '#FF6600';     // 25%: オレンジ
    if (percentage <= 50) return '#FFCC00';     // 50%: 黄色
    if (percentage <= 75) return '#99CC00';     // 75%: 黄緑
    return '#00CC00';                           // 100%: 緑
}

/**
 * パーティ画面用ポケモンカードコンポーネント
 * - ポケモン画像、名前、レベル、タイプ、HP、HPバーを表示
 * - 参照画像のデザインを忠実に再現
 */
export default function PartyPokemonCard({
    pokemon,
    species,
}: PartyPokemonCardProps) {
    // タイプのEnum値を文字列に変換
    const type1String = convertTypeEnumToString(species.type1);
    const type2String = species.type2 != null ? convertTypeEnumToString(species.type2) : undefined;

    const typeColors = getTypeColors(type1String);

    // HP計算（現在は最大HP = 現在HP）
    const maxHp = calcHp(pokemon.level, species.baseHp);
    const currentHp = maxHp; // パーティでは常に満タン
    const hpPercentage = (currentHp / maxHp) * 100;
    const hpBarColor = getHpBarColor(hpPercentage);

    return (
        <div
            className="rounded-lg p-1 flex flex-col items-center gap-0.5"
            style={{
                backgroundColor: '#2a2a2a',
                border: `2px solid ${typeColors.border}`,
            }}
        >
            {/* ポケモン画像 */}
            <div className="relative w-10 h-10 flex items-center justify-center">
                <Image
                    src={species.frontImage}
                    alt={species.name}
                    width={40}
                    height={40}
                    className="object-contain"
                    unoptimized
                />
            </div>

            {/* 名前とレベル */}
            <div className="text-center">
                <p
                    className="text-xs font-bold leading-tight"
                    style={{ color: typeColors.border }}
                >
                    {species.name}
                </p>
                <p className="text-[10px] text-gray-300 leading-tight">
                    Lv.{pokemon.level}
                </p>
            </div>

            {/* タイプ */}
            <div className="flex gap-0.5">
                <span
                    className="text-[9px] px-1.5 py-0.5 rounded"
                    style={{
                        backgroundColor: typeColors.border,
                        color: '#ffffff',
                    }}
                >
                    {type1String}
                </span>
                {type2String && (
                    <span
                        className="text-[9px] px-1.5 py-0.5 rounded"
                        style={{
                            backgroundColor: getTypeColors(type2String).border,
                            color: '#ffffff',
                        }}
                    >
                        {type2String}
                    </span>
                )}
            </div>

            {/* HP表示 */}
            <div className="w-full mt-0.5">
                <div className="flex justify-between text-[9px] text-gray-300 mb-0.5">
                    <span>HP</span>
                    <span>{currentHp}/{maxHp}</span>
                </div>
                {/* HPバー */}
                <div
                    className="w-full h-1.5 rounded-full overflow-hidden"
                    style={{ backgroundColor: '#444444' }}
                >
                    <div
                        className="h-full rounded-full transition-all duration-300"
                        style={{
                            width: `${hpPercentage}%`,
                            backgroundColor: hpBarColor,
                        }}
                    />
                </div>
            </div>
        </div>
    );
}
