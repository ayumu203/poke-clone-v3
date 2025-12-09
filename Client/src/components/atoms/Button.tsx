interface ButtonProps {
    children: React.ReactNode;
    onClick?: () => void;
    type?: 'button' | 'submit';
    variant?: 'primary' | 'secondary' | 'google';
    fullWidth?: boolean;
    disabled?: boolean;
    icon?: React.ReactNode;
}

/**
 * ドット調デザインのボタンコンポーネント
 * - ピクセル風のデザイン
 * - バリエーション: primary, secondary, google
 * - ホバーエフェクト
 * - アイコンサポート
 */
export default function Button({
    children,
    onClick,
    type = 'button',
    variant = 'primary',
    fullWidth = false,
    disabled = false,
    icon,
}: ButtonProps) {
    const baseClasses = 'px-4 py-2 border-2 font-bold transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed';

    const variantClasses = {
        primary: 'bg-blue-600 border-blue-800 text-white hover:bg-blue-700',
        secondary: 'bg-gray-600 border-gray-800 text-white hover:bg-gray-700',
        google: 'bg-red-600 border-red-800 text-white hover:bg-red-700',
    };

    const widthClass = fullWidth ? 'w-full' : '';

    return (
        <button
            type={type}
            onClick={onClick}
            disabled={disabled}
            className={`${baseClasses} ${variantClasses[variant]} ${widthClass} flex items-center justify-center gap-2`}
        >
            {icon && <span className="text-xl">{icon}</span>}
            {children}
        </button>
    );
}
