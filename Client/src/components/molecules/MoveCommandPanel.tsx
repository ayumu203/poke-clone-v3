"use client";

import type { Move } from "@/types/pokemon";
import { getTypeColors } from "@/utils/typeColors";
import { convertTypeEnumToString } from "@/utils/typeEnum";

interface MoveCommandPanelProps {
    /** 技リスト */
    moves: Move[];
    /** 現在のPP状態（オプション、なければmove.ppを使用） */
    currentPPs?: number[];
    /** 技選択時のコールバック */
    onMoveSelect: (moveId: number) => void;
    /** 戻るボタン押下時のコールバック */
    onBack: () => void;
    /** 戻るボタンのテキスト */
    backText?: string;
    /** 無効化状態 */
    disabled?: boolean;
}

/**
 * 技選択パネル（4つの技ボタン + 戻るボタン）
 */
export default function MoveCommandPanel({
    moves,
    currentPPs,
    onMoveSelect,
    onBack,
    backText = "もどる",
    disabled = false,
}: MoveCommandPanelProps) {
    return (
        <div className="flex h-full flex-col gap-2 p-2">
            {/* 技ボタングリッド（2x2） */}
            <div className="grid flex-1 grid-cols-2 gap-2">
                {[0, 1, 2, 3].map((index) => {
                    const move = moves[index];
                    if (!move) {
                        // 空スロット
                        return (
                            <div
                                key={index}
                                className="flex items-center justify-center rounded-lg border border-gray-600"
                                style={{ backgroundColor: "rgba(50, 50, 50, 0.8)" }}
                            >
                                <span className="text-gray-500">---</span>
                            </div>
                        );
                    }

                    const currentPP = currentPPs?.[index] ?? move.pp;
                    const maxPP = move.pp;
                    const isDisabled = disabled || currentPP <= 0;
                    const typeString =
                        typeof move.type === "number" ? convertTypeEnumToString(move.type) : move.type;
                    const typeColors = getTypeColors(typeString);

                    return (
                        <button
                            key={move.moveId}
                            onClick={() => !isDisabled && onMoveSelect(move.moveId)}
                            disabled={isDisabled}
                            className={`flex flex-col items-start justify-center gap-1 rounded-lg border-2 p-2 text-left transition-all ${isDisabled
                                    ? "cursor-not-allowed opacity-50"
                                    : "cursor-pointer hover:scale-[1.02] hover:brightness-110"
                                }`}
                            style={{
                                backgroundColor: "rgba(30, 30, 30, 0.9)",
                                borderColor: typeColors.border,
                            }}
                        >
                            {/* 技名 */}
                            <span className="text-sm font-bold text-white md:text-base">{move.name}</span>

                            {/* タイプとPP */}
                            <div className="flex w-full items-center justify-between">
                                <span
                                    className="rounded px-1.5 py-0.5 text-xs font-bold text-white"
                                    style={{ backgroundColor: typeColors.border }}
                                >
                                    {typeString}
                                </span>
                                <span className="text-xs text-gray-300">
                                    PP {currentPP}/{maxPP}
                                </span>
                            </div>
                        </button>
                    );
                })}
            </div>

            {/* 戻るボタン */}
            <button
                onClick={onBack}
                disabled={disabled}
                className="rounded-lg bg-gray-600 py-2 text-center font-bold text-white transition-colors hover:bg-gray-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
                {backText}
            </button>
        </div>
    );
}
