"use client";

import Image from "next/image";

import TypeBadge from "@/components/atoms/TypeBadge";
import type { StarterOption } from "@/types/starter";
import { getTypeColors } from "@/utils/typeColors";
import { convertTypeEnumToString } from "@/utils/typeEnum";

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
      className="w-full rounded-lg bg-white p-4 transition-all duration-200 hover:shadow-lg disabled:cursor-not-allowed disabled:opacity-50"
      style={{
        borderWidth: "4px",
        borderStyle: "solid",
        borderColor: typeColors.border,
        backgroundColor: `${typeColors.background}80`, // 50% opacity
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
        <div className="flex flex-wrap justify-center gap-2">
          <TypeBadge type={type1String} size="md" className="rounded-full px-3 py-1 font-semibold" />
          {type2String && (
            <TypeBadge type={type2String} size="md" className="rounded-full px-3 py-1 font-semibold" />
          )}
        </div>

        {/* 説明文 */}
        {description && (
          <p
            className="text-center text-sm leading-relaxed"
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
