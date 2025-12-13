'use client';

import Image from 'next/image';

import type { StarterOption } from '@/types/starter';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';

interface StarterPokemonCardProps {
    pokemon: StarterOption;
    description?: string;
    onClick: () => void;
    disabled?: boolean;
}

/**
 * スターターポケモンカードコンポーネント
 * - タイプに応じた枠線色
 * - 元の画像サイズでポケモン画像を表示
 * - 説明文の表示
 * - ホバー効果
 */
export default function StarterPokemonCard({
    pokemon,
    description,
    onClick,
    disabled = false,
}: StarterPokemonCardProps) {
    // タイプのEnum値を文字列に変換
    const type1String = convertTypeEnumToString(pokemon.type1);
    const type2String = pokemon.type2 ? convertTypeEnumToString(pokemon.type2) : undefined;

    const typeColors = getTypeColors(type1String);

    return (
        <button
            onClick={onClick}
            disabled={disabled}
            className="w-full bg-white rounded-lg p-4 transition-all duration-200 hover:shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
            style={{
                borderWidth: '4px',
                borderStyle: 'solid',
                borderColor: typeColors.border,
                backgroundColor: typeColors.background,
            }}
        >
            <div className="flex flex-col items-center gap-3">
                {/* ポケモン画像 - 元のサイズで表示 */}
                {pokemon.frontImage && (
                    <div className="relative">
                        <Image
                            src={pokemon.frontImage}
                            alt={pokemon.name}
                            width={96}
                            height={96}
                            className="object-contain"
                            unoptimized
                        />
                    </div>
                )}

                {/* ポケモン名 */}
                <h3
                    className="text-xl font-bold"
                    style={{
                        color: typeColors.text,
                    }}
                >
                    {pokemon.name}
                </h3>

                {/* タイプ表示 */}
                <div className="flex gap-2 flex-wrap justify-center">
                    <span
                        className="px-3 py-1 rounded-full text-sm font-semibold"
                        style={{
                            backgroundColor: typeColors.border,
                            color: 'white',
                        }}
                    >
                        {type1String}
                    </span>
                    {type2String && (
                        <span
                            className="px-3 py-1 rounded-full text-sm font-semibold"
                            style={{
                                backgroundColor: getTypeColors(type2String).border,
                                color: 'white',
                            }}
                        >
                            {type2String}
                        </span>
                    )}
                </div>

                {/* 説明文 */}
                {description && (
                    <p
                        className="text-sm leading-relaxed text-center"
                        style={{
                            color: typeColors.text,
                        }}
                    >
                        {description}
                    </p>
                )}
            </div>
        </button>
    );
}
