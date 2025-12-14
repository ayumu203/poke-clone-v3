"use client";

interface BattleCommandPanelProps {
    /** たたかうボタン押下時 */
    onAttack: () => void;
    /** つかまえるボタン押下時 */
    onCatch: () => void;
    /** にげるボタン押下時 */
    onEscape: () => void;
    /** ボタンテキスト */
    texts?: {
        attack?: string;
        catch?: string;
        escape?: string;
    };
    /** 無効化状態 */
    disabled?: boolean;
}

/**
 * メインコマンド選択パネル
 * - たたかう
 * - つかまえる
 * - にげる
 */
export default function BattleCommandPanel({
    onAttack,
    onCatch,
    onEscape,
    texts,
    disabled = false,
}: BattleCommandPanelProps) {
    const attackText = texts?.attack || "たたかう";
    const catchText = texts?.catch || "つかまえる";
    const escapeText = texts?.escape || "にげる";

    const buttonClass =
        "flex-1 rounded-lg py-4 text-center text-lg font-bold text-white transition-all disabled:cursor-not-allowed disabled:opacity-50";

    return (
        <div className="flex h-full flex-col gap-3 p-3">
            {/* たたかう */}
            <button
                onClick={onAttack}
                disabled={disabled}
                className={`${buttonClass} bg-red-600 hover:bg-red-500`}
            >
                {attackText}
            </button>

            {/* つかまえる */}
            <button
                onClick={onCatch}
                disabled={disabled}
                className={`${buttonClass} bg-green-600 hover:bg-green-500`}
            >
                {catchText}
            </button>

            {/* にげる */}
            <button
                onClick={onEscape}
                disabled={disabled}
                className={`${buttonClass} bg-gray-600 hover:bg-gray-500`}
            >
                {escapeText}
            </button>
        </div>
    );
}
