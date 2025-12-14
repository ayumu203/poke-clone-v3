'use client';


import type { Pokemon } from '@/types/pokemon';
import type { PokemonSpecies } from '@/types/pokemonSpecies';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';
import { calcHp } from '@/utils/statCalculator';

interface PartyPokemonCardProps {
    pokemon: Pokemon;
    species: PokemonSpecies;
    onClick?: () => void;
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
 * 左側: 画像 + レベル
 * 右側: 名前・HPバー・HP実数値・タイプ
 */
export default function PartyPokemonCard({
    pokemon,
    species,
    onClick,
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
        <button
            onClick={onClick}
            className="rounded-lg p-2 flex flex-col md:flex-row items-center md:items-end text-left transition-transform hover:scale-105 cursor-pointer"
            style={{
                backgroundColor: '#2a2a2a',
                border: `2px solid ${typeColors.border}`,
            }}
        >
            {/* 上/左側: ポケモン画像 + レベル */}
            <div className="flex flex-col items-center">
                {/* eslint-disable-next-line @next/next/no-img-element */}
                <img
                    src={species.frontImage}
                    alt={species.name}
                    className="object-contain w-16 h-16 md:w-auto md:h-auto"
                />
                <p className="text-xl text-gray-300">
                    Lv.{pokemon.level}
                </p>
            </div>

            {/* 下/右側: 名前・HPバー・HP・タイプ */}
            <div className="flex-1 flex flex-col items-center md:items-start md:pl-1 gap-0.5 w-full">
                {/* 名前 */}
                <p
                    className="text-xl font-bold leading-tight"
                    style={{ color: typeColors.border }}
                >
                    {species.name}
                </p>

                {/* HPバー */}
                <div
                    className="w-full mb-2 h-1.5 rounded-full overflow-hidden"
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

                {/* HP実数値 */}
                <p className="text-lg text-gray-300">
                    HP: {currentHp}
                </p>

                {/* タイプ（横並び） */}
                <div className="flex gap-0.5 md:gap-1">
                    <span
                        className="text-xs md:text-sm px-1 py-0.5 rounded md:whitespace-nowrap"
                        style={{
                            backgroundColor: typeColors.border,
                            color: '#ffffff',
                        }}
                    >
                        {type1String}
                    </span>
                    {type2String && (
                        <span
                            className="text-xs md:text-sm px-1 py-0.5 rounded md:whitespace-nowrap"
                            style={{
                                backgroundColor: getTypeColors(type2String).border,
                                color: '#ffffff',
                            }}
                        >
                            {type2String}
                        </span>
                    )}
                </div>
            </div>
        </button>
    );
}

