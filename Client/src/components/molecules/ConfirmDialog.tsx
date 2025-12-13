'use client';

interface ConfirmDialogProps {
    isOpen: boolean;
    title?: string;
    message: string;
    confirmText: string;
    cancelText: string;
    onConfirm: () => void;
    onCancel: () => void;
    isLoading?: boolean;
    variant?: 'danger' | 'default';
}

/**
 * 確認ダイアログコンポーネント
 * - モーダル形式
 * - 確認・キャンセルボタン
 */
export default function ConfirmDialog({
    isOpen,
    title,
    message,
    confirmText,
    cancelText,
    onConfirm,
    onCancel,
    isLoading = false,
    variant = 'default',
}: ConfirmDialogProps) {
    if (!isOpen) return null;

    const confirmButtonClass = variant === 'danger'
        ? 'bg-red-600 hover:bg-red-500'
        : 'bg-blue-600 hover:bg-blue-500';

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center">
            {/* オーバーレイ */}
            <div
                className="absolute inset-0 bg-black/60"
                onClick={onCancel}
            />

            {/* ダイアログ */}
            <div className="relative bg-gray-800 rounded-lg p-6 max-w-sm mx-4 shadow-xl border border-gray-600">
                {title && (
                    <h3 className="text-lg font-bold text-white mb-3">
                        {title}
                    </h3>
                )}
                <p className="text-gray-200 mb-6">
                    {message}
                </p>
                <div className="flex justify-end gap-3">
                    <button
                        onClick={onCancel}
                        disabled={isLoading}
                        className="px-4 py-2 bg-gray-600 hover:bg-gray-500 disabled:bg-gray-700 text-white rounded transition-colors"
                    >
                        {cancelText}
                    </button>
                    <button
                        onClick={onConfirm}
                        disabled={isLoading}
                        className={`px-4 py-2 ${confirmButtonClass} disabled:bg-gray-500 text-white rounded transition-colors`}
                    >
                        {isLoading ? '...' : confirmText}
                    </button>
                </div>
            </div>
        </div>
    );
}
