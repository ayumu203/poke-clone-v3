'use client';

import type { Pokemon } from '@/types/pokemon';
import type { PokemonSpecies } from '@/types/pokemonSpecies';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';
import { calcHp, calcStat } from '@/utils/statCalculator';

interface PokemonDetailTopProps {
    pokemon: Pokemon;
    species: PokemonSpecies;
    texts: {
        attack: string;
        defense: string;
        spAttack: string;
        spDefense: string;
        speed: string;
    };
}

/**
 * ポケモン詳細の上画面コンポーネント
 * - 背景画像の上に半透明レイヤー
 * - ポケモン画像、タイプ、レベル、名前、ステータスを表示
 * - デスクトップでは要素を2倍サイズにして中央寄せ
 */
export default function PokemonDetailTop({
    pokemon,
    species,
    texts,
}: PokemonDetailTopProps) {
    const type1String = convertTypeEnumToString(species.type1);
    const type2String = species.type2 != null ? convertTypeEnumToString(species.type2) : undefined;
    const type1Colors = getTypeColors(type1String);
    const type2Colors = type2String ? getTypeColors(type2String) : null;

    // ステータス計算
    const hp = calcHp(pokemon.level, species.baseHp);
    const attack = calcStat(pokemon.level, species.baseAttack);
    const defense = calcStat(pokemon.level, species.baseDefence);
    const spAttack = calcStat(pokemon.level, species.baseSpecialAttack);
    const spDefense = calcStat(pokemon.level, species.baseSpecialDefence);
    const speed = calcStat(pokemon.level, species.baseSpeed);

    return (
        <div className="absolute inset-0 flex items-center justify-center">
            {/* 半透明オーバーレイ */}
            <div className="absolute inset-0 bg-black/40" />

            {/* コンテンツ - デスクトップでは中央寄せ */}
            <div className="relative z-10 flex w-full h-full p-3 md:w-auto md:h-auto md:max-w-[80%] md:max-h-[90%]">
                {/* 左側: ポケモン画像 */}
                <div className="flex-1 flex items-center justify-center md:flex-none md:w-48">
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img
                        src={species.frontImage}
                        alt={species.name}
                        className="max-h-full object-contain drop-shadow-lg md:scale-150"
                        style={{ imageRendering: 'pixelated' }}
                    />
                </div>

                {/* 右側: 情報 */}
                <div className="flex-1 flex flex-col justify-between py-2 md:flex-none md:ml-8">
                    {/* 上部: タイプ・名前・レベル */}
                    <div className="flex flex-col items-end gap-1 md:gap-2">
                        {/* タイプバッジ */}
                        <div className="flex gap-1 md:gap-2">
                            <span
                                className="text-xs md:text-base px-2 md:px-4 py-0.5 md:py-1 rounded font-bold"
                                style={{ backgroundColor: type1Colors.border, color: '#fff' }}
                            >
                                {type1String}
                            </span>
                            {type2String && type2Colors && (
                                <span
                                    className="text-xs md:text-base px-2 md:px-4 py-0.5 md:py-1 rounded font-bold"
                                    style={{ backgroundColor: type2Colors.border, color: '#fff' }}
                                >
                                    {type2String}
                                </span>
                            )}
                        </div>
                        {/* 名前・レベル */}
                        <p className="text-xl md:text-3xl font-bold text-white drop-shadow">
                            {species.name}
                        </p>
                        <p className="text-lg md:text-2xl text-gray-200">
                            Lv. {pokemon.level}
                        </p>
                    </div>

                    {/* 下部: ステータス */}
                    <div
                        className="bg-black/50 rounded p-2 md:p-4 text-sm md:text-lg mt-2 md:mt-4"
                        style={{
                            display: 'grid',
                            gridTemplateColumns: 'auto 1fr auto 1fr',
                            gap: '2px 8px',
                            minWidth: '240px',
                        }}
                    >
                        <span className="text-gray-300">HP</span>
                        <span className="text-white font-bold">{hp}</span>
                        <span className="text-gray-300">{texts.attack}</span>
                        <span className="text-white font-bold">{attack}</span>

                        <span className="text-gray-300">{texts.defense}</span>
                        <span className="text-white font-bold">{defense}</span>
                        <span className="text-gray-300">{texts.spAttack}</span>
                        <span className="text-white font-bold">{spAttack}</span>

                        <span className="text-gray-300">{texts.spDefense}</span>
                        <span className="text-white font-bold">{spDefense}</span>
                        <span className="text-gray-300">{texts.speed}</span>
                        <span className="text-white font-bold">{speed}</span>
                    </div>
                </div>
            </div>
        </div>
    );
}
