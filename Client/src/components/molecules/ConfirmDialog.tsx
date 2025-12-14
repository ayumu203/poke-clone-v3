"use client";

interface ConfirmDialogProps {
  isOpen: boolean;
  title?: string;
  message: string;
  confirmText: string;
  cancelText: string;
  onConfirm: () => void;
  onCancel: () => void;
  isLoading?: boolean;
  variant?: "danger" | "default";
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
  variant = "default",
}: ConfirmDialogProps) {
  if (!isOpen) return null;

  const confirmButtonClass =
    variant === "danger" ? "bg-red-600 hover:bg-red-500" : "bg-blue-600 hover:bg-blue-500";

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* オーバーレイ */}
      <div className="absolute inset-0 bg-black/60" onClick={onCancel} />

      {/* ダイアログ */}
      <div className="relative mx-4 max-w-sm rounded-lg border border-gray-600 bg-gray-800 p-6 shadow-xl">
        {title && <h3 className="mb-3 text-lg font-bold text-white">{title}</h3>}
        <p className="mb-6 text-gray-200">{message}</p>
        <div className="flex justify-end gap-3">
          <button
            onClick={onCancel}
            disabled={isLoading}
            className="rounded bg-gray-600 px-4 py-2 text-white transition-colors hover:bg-gray-500 disabled:bg-gray-700"
          >
            {cancelText}
          </button>
          <button
            onClick={onConfirm}
            disabled={isLoading}
            className={`px-4 py-2 ${confirmButtonClass} rounded text-white transition-colors disabled:bg-gray-500`}
          >
            {isLoading ? "..." : confirmText}
          </button>
        </div>
      </div>
    </div>
  );
}
