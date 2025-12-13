'use client';

import { FaArrowLeft } from 'react-icons/fa';

import type { Pokemon, Move } from '@/types/pokemon';
import MoveSlot from '@/components/atoms/MoveSlot';

interface PokemonDetailBottomProps {
    pokemon: Pokemon;
    texts: {
        release: string;
        back: string;
    };
    onBack: () => void;
    onRelease: () => void;
    isReleasing?: boolean;
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
            <div className="flex-1 p-3 flex flex-col gap-2 md:gap-3">
                {moves.slice(0, 4).map((move, index) => (
                    <MoveSlot key={move?.moveId || `empty-${index}`} move={move} />
                ))}
            </div>

            {/* フッター */}
            <div className="bg-gray-800 p-2 flex justify-center gap-4 md:gap-6">
                {/* 戻るボタン */}
                <button
                    onClick={onBack}
                    className="flex items-center gap-2 px-4 py-2 md:py-4 bg-gray-600 hover:bg-gray-500 text-white rounded transition-colors"
                >
                    <FaArrowLeft />
                    <span>{texts.back}</span>
                </button>
                {/* 逃がすボタン */}
                <button
                    onClick={onRelease}
                    disabled={isReleasing}
                    className="flex items-center gap-2 px-4 py-2 md:py-4 bg-red-600 hover:bg-red-500 disabled:bg-gray-500 text-white rounded transition-colors"
                >
                    {isReleasing ? '...' : texts.release}
                </button>
            </div>
        </div>
    );
}
