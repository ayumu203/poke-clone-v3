'use client';

import type { Move } from '@/types/pokemon';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';

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
                className="h-12 md:h-14 border border-cyan-700/50 rounded"
                style={{ backgroundColor: 'rgba(0, 40, 80, 0.6)' }}
            />
        );
    }

    // typeがnumber型の場合も考慮
    const typeString = typeof move.type === 'number'
        ? convertTypeEnumToString(move.type)
        : move.type;
    const typeColors = getTypeColors(typeString);

    return (
        <div
            className="h-12 md:h-14 border border-cyan-700/50 rounded flex items-center px-3 gap-3"
            style={{ backgroundColor: 'rgba(0, 40, 80, 0.6)' }}
        >
            {/* タイプ */}
            <span
                className="text-xs md:text-sm px-2 py-0.5 rounded font-bold shrink-0"
                style={{ backgroundColor: typeColors.border, color: '#fff' }}
            >
                {typeString}
            </span>
            {/* 技名 */}
            <span className="text-white font-bold flex-1 truncate text-sm md:text-base">
                {move.name}
            </span>
            {/* PP */}
            <span className="text-gray-300 text-sm shrink-0">
                PP {move.pp}/{move.pp}
            </span>
        </div>
    );
}
