interface InputProps {
    type?: 'text' | 'email' | 'password';
    placeholder?: string;
    value: string;
    onChange: (value: string) => void;
    required?: boolean;
    minLength?: number;
}

/**
 * ピクセル風の入力フィールドコンポーネント
 * - ドット調デザイン
 * - type（text, email, password）対応
 */
export default function Input({
    type = 'text',
    placeholder,
    value,
    onChange,
    required = false,
    minLength,
}: InputProps) {
    return (
        <input
            type={type}
            placeholder={placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
            required={required}
            minLength={minLength}
            className="w-full px-3 py-2 border-2 border-gray-800 bg-white text-black focus:outline-none focus:border-blue-600 font-sans"
        />
    );
}
