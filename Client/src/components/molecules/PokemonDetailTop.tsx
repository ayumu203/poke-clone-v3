"use client";

import Image from "next/image";

import TypeBadge from "@/components/atoms/TypeBadge";
import type { Pokemon } from "@/types/pokemon";
import type { PokemonSpecies } from "@/types/pokemonSpecies";
import { calcHp, calcStat } from "@/utils/statCalculator";
import { convertTypeEnumToString } from "@/utils/typeEnum";

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
export default function PokemonDetailTop({ pokemon, species, texts }: PokemonDetailTopProps) {
  const type1String = convertTypeEnumToString(species.type1);
  const type2String = species.type2 != null ? convertTypeEnumToString(species.type2) : undefined;

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
      <div className="relative z-10 flex h-full w-full p-3 md:h-auto md:max-h-[90%] md:w-auto md:max-w-[80%]">
        {/* 左側: ポケモン画像 */}
        <div className="flex flex-1 items-center justify-center md:w-48 md:flex-none">
          <Image
            src={species.frontImage}
            alt={species.name}
            width={144}
            height={144}
            className="max-h-full object-contain drop-shadow-lg md:scale-150"
            style={{ imageRendering: "pixelated" }}
            unoptimized
          />
        </div>

        {/* 右側: 情報 */}
        <div className="flex flex-1 flex-col justify-between py-2 md:ml-8 md:flex-none">
          {/* 上部: タイプ・名前・レベル */}
          <div className="flex flex-col items-end gap-1 md:gap-2">
            {/* タイプバッジ */}
            <div className="flex gap-1 md:gap-2">
              <TypeBadge
                type={type1String}
                size="sm"
                className="md:px-4 md:py-1 md:text-base"
              />
              {type2String && (
                <TypeBadge
                  type={type2String}
                  size="sm"
                  className="md:px-4 md:py-1 md:text-base"
                />
              )}
            </div>
            {/* 名前・レベル */}
            <p className="text-xl font-bold text-white drop-shadow md:text-3xl">{species.name}</p>
            <p className="text-lg text-gray-200 md:text-2xl">Lv. {pokemon.level}</p>
          </div>

          {/* 下部: ステータス */}
          <div
            className="mt-2 rounded bg-black/50 p-2 text-sm md:mt-4 md:p-4 md:text-lg"
            style={{
              display: "grid",
              gridTemplateColumns: "auto 1fr auto 1fr",
              gap: "2px 8px",
              minWidth: "240px",
            }}
          >
            <span className="text-gray-300">HP</span>
            <span className="font-bold text-white">{hp}</span>
            <span className="text-gray-300">{texts.attack}</span>
            <span className="font-bold text-white">{attack}</span>

            <span className="text-gray-300">{texts.defense}</span>
            <span className="font-bold text-white">{defense}</span>
            <span className="text-gray-300">{texts.spAttack}</span>
            <span className="font-bold text-white">{spAttack}</span>

            <span className="text-gray-300">{texts.spDefense}</span>
            <span className="font-bold text-white">{spDefense}</span>
            <span className="text-gray-300">{texts.speed}</span>
            <span className="font-bold text-white">{speed}</span>
          </div>
        </div>
      </div>
    </div>
  );
}
