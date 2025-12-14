"use client";

import type { Move } from "@/types/pokemon";
import { getTypeColors } from "@/utils/typeColors";
import { convertTypeEnumToString } from "@/utils/typeEnum";

interface MoveSlotProps {
  move?: Move;
}

/**
 * 技1つを表示するコンポーネント
 * - 技がない場合は空のスロットを表示
 * - デスクトップでは高さを増加
 */
export default function MoveSlot({ move }: MoveSlotProps) {
  if (!move) {
    return (
      <div
        className="h-12 rounded border border-cyan-700/50 md:h-14"
        style={{ backgroundColor: "rgba(0, 40, 80, 0.6)" }}
      />
    );
  }

  // typeがnumber型の場合も考慮
  const typeString = typeof move.type === "number" ? convertTypeEnumToString(move.type) : move.type;
  const typeColors = getTypeColors(typeString);

  return (
    <div
      className="flex h-12 items-center gap-3 rounded border border-cyan-700/50 px-3 md:h-14"
      style={{ backgroundColor: "rgba(0, 40, 80, 0.6)" }}
    >
      {/* タイプ */}
      <span
        className="shrink-0 rounded px-2 py-0.5 text-xs font-bold md:text-sm"
        style={{ backgroundColor: typeColors.border, color: "#fff" }}
      >
        {typeString}
      </span>
      {/* 技名 */}
      <span className="flex-1 truncate text-sm font-bold text-white md:text-base">{move.name}</span>
      {/* PP */}
      <span className="shrink-0 text-sm text-gray-300">
        PP {move.pp}/{move.pp}
      </span>
    </div>
  );
}
