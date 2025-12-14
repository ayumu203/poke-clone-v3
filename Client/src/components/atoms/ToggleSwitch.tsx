interface ToggleSwitchProps {
  enabled: boolean;
  onToggle: () => void;
  label?: string;
}

/**
 * トグルスイッチコンポーネント
 * - ON/OFFの切り替えが可能
 * - スイッチの色が状態に応じて変化（緑/灰色）
 * - アクセシビリティ対応
 */
export default function ToggleSwitch({ enabled, onToggle, label }: ToggleSwitchProps) {
  return (
    <div className="flex items-center gap-2">
      {label && <span className="text-sm">{label}</span>}
      <button
        onClick={onToggle}
        className={`h-6 w-10 rounded-full transition-colors ${
          enabled ? "bg-green-400" : "bg-gray-400"
        } relative`}
        aria-label={label || "トグルスイッチ"}
        role="switch"
        aria-checked={enabled}
      >
        <span
          className={`absolute top-1 h-4 w-4 rounded-full bg-white transition-transform ${
            enabled ? "right-1" : "left-1"
          }`}
        />
      </button>
    </div>
  );
}
