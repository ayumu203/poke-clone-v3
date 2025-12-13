'use client';

import { FaArrowLeft } from 'react-icons/fa';

import type { Pokemon, Move } from '@/types/pokemon';
import type { PokemonSpecies } from '@/types/pokemonSpecies';
import { getTypeColors } from '@/utils/typeColors';
import { convertTypeEnumToString } from '@/utils/typeEnum';

interface PokemonDetailBottomProps {
    pokemon: Pokemon;
    species: PokemonSpecies;
    texts: {
        release: string;
        back: string;
    };
    onBack: () => void;
    onRelease: () => void;
    isReleasing?: boolean;
}

/**
 * 技1つを表示するコンポーネント
 */
function MoveSlot({ move }: { move?: Move }) {
    if (!move) {
        return (
            <div
                className="h-12 border border-cyan-700/50 rounded"
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
            className="h-12 border border-cyan-700/50 rounded flex items-center px-3 gap-3"
            style={{ backgroundColor: 'rgba(0, 40, 80, 0.6)' }}
        >
            {/* タイプ */}
            <span
                className="text-xs px-2 py-0.5 rounded font-bold shrink-0"
                style={{ backgroundColor: typeColors.border, color: '#fff' }}
            >
                {typeString}
            </span>
            {/* 技名 */}
            <span className="text-white font-bold flex-1 truncate">
                {move.name}
            </span>
            {/* PP */}
            <span className="text-gray-300 text-sm shrink-0">
                PP {move.pp}/{move.pp}
            </span>
        </div>
    );
}

/**
 * ポケモン詳細の下画面コンポーネント
 * - 技リスト（4つ）
 * - 戻るボタン、逃がすボタン
 * - 格子模様背景
 */
export default function PokemonDetailBottom({
    pokemon,
    texts,
    onBack,
    onRelease,
    isReleasing = false,
}: PokemonDetailBottomProps) {
    // 技を4つのスロットにpadding
    const moves = [...(pokemon.moves || [])];
    while (moves.length < 4) {
        moves.push(undefined as unknown as Move);
    }

    return (
        <div className="flex flex-col h-full">
            {/* 技リスト */}
            <div className="flex-1 p-3 flex flex-col gap-2">
                {moves.slice(0, 4).map((move, index) => (
                    <MoveSlot key={move?.moveId || `empty-${index}`} move={move} />
                ))}
            </div>

            {/* フッター */}
            <div className="bg-gray-800 p-2 flex justify-center gap-4">
                {/* 戻るボタン */}
                <button
                    onClick={onBack}
                    className="flex items-center gap-2 px-4 py-2 bg-gray-600 hover:bg-gray-500 text-white rounded transition-colors"
                >
                    <FaArrowLeft />
                    <span>{texts.back}</span>
                </button>
                {/* 逃がすボタン */}
                <button
                    onClick={onRelease}
                    disabled={isReleasing}
                    className="flex items-center gap-2 px-4 py-2 bg-red-600 hover:bg-red-500 disabled:bg-gray-500 text-white rounded transition-colors"
                >
                    {isReleasing ? '...' : texts.release}
                </button>
            </div>
        </div>
    );
}
