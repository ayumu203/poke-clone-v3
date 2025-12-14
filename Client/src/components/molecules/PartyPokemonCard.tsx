"use client";

import Image from "next/image";

import HPBar from "@/components/atoms/HPBar";
import TypeBadge from "@/components/atoms/TypeBadge";
import type { Pokemon } from "@/types/pokemon";
import type { PokemonSpecies } from "@/types/pokemonSpecies";
import { calcHp } from "@/utils/statCalculator";
import { getTypeColors } from "@/utils/typeColors";
import { convertTypeEnumToString } from "@/utils/typeEnum";

interface PartyPokemonCardProps {
  pokemon: Pokemon;
  species: PokemonSpecies;
  onClick?: () => void;
}

/**
 * パーティ画面用ポケモンカードコンポーネント
 * 左側: 画像 + レベル
 * 右側: 名前・HPバー・HP実数値・タイプ
 */
export default function PartyPokemonCard({ pokemon, species, onClick }: PartyPokemonCardProps) {
  // タイプのEnum値を文字列に変換
  const type1String = convertTypeEnumToString(species.type1);
  const type2String = species.type2 != null ? convertTypeEnumToString(species.type2) : undefined;

  const typeColors = getTypeColors(type1String);

  // HP計算（現在は最大HP = 現在HP）
  const maxHp = calcHp(pokemon.level, species.baseHp);
  const currentHp = maxHp; // パーティでは常に満タン

  return (
    <button
      onClick={onClick}
      className="flex cursor-pointer flex-col items-center rounded-lg p-2 text-left transition-transform hover:scale-105 md:flex-row md:items-end"
      style={{
        backgroundColor: "#2a2a2a",
        border: `2px solid ${typeColors.border}`,
      }}
    >
      {/* 上/左側: ポケモン画像 + レベル */}
      <div className="flex flex-col items-center">
        <Image
          src={species.frontImage}
          alt={species.name}
          width={96}
          height={96}
          className="h-16 w-16 object-contain md:h-auto md:w-auto"
          unoptimized
        />
        <p className="text-xl text-gray-300">Lv.{pokemon.level}</p>
      </div>

      {/* 下/右側: 名前・HPバー・HP・タイプ */}
      <div className="flex w-full flex-1 flex-col items-center gap-0.5 md:items-start md:pl-1">
        {/* 名前 */}
        <p className="text-xl leading-tight font-bold" style={{ color: typeColors.border }}>
          {species.name}
        </p>

        {/* HPバー */}
        <HPBar currentHp={currentHp} maxHp={maxHp} className="mb-2 w-full" />

        {/* HP実数値 */}
        <p className="text-lg text-gray-300">HP: {currentHp}</p>

        {/* タイプ（横並び） */}
        <div className="flex gap-0.5 md:gap-1">
          <TypeBadge type={type1String} size="sm" className="md:text-sm md:whitespace-nowrap" />
          {type2String && (
            <TypeBadge type={type2String} size="sm" className="md:text-sm md:whitespace-nowrap" />
          )}
        </div>
      </div>
    </button>
  );
}
