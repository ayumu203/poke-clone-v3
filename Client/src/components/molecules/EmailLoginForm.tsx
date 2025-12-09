'use client';

import { useState } from 'react';

import Button from '@/components/atoms/Button';
import Input from '@/components/atoms/Input';

type FormMode = 'login' | 'signup';

interface EmailLoginFormProps {
    onLogin: (email: string, password: string) => Promise<void>;
    onSignUp: (email: string, password: string) => Promise<void>;
    onCancel: () => void;
    error?: string;
}

/**
 * メールログイン/新規登録フォームコンポーネント
 * - ログインと新規登録のモード切り替え
 * - Email、Passwordの入力フィールド
 * - バリデーション
 * - エラー表示
 */
export default function EmailLoginForm({ onLogin, onSignUp, onCancel, error }: EmailLoginFormProps) {
    const [mode, setMode] = useState<FormMode>('login');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSubmitting(true);
        try {
            if (mode === 'login') {
                await onLogin(email, password);
            } else {
                await onSignUp(email, password);
            }
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 p-4">
            {/* モード切り替えタブ */}
            <div className="flex gap-2 mb-4">
                <button
                    type="button"
                    onClick={() => setMode('login')}
                    className={`flex-1 py-2 px-4 font-bold border-2 ${mode === 'login'
                            ? 'bg-blue-600 border-blue-800 text-white'
                            : 'bg-gray-300 border-gray-500 text-gray-700'
                        }`}
                >
                    ログイン
                </button>
                <button
                    type="button"
                    onClick={() => setMode('signup')}
                    className={`flex-1 py-2 px-4 font-bold border-2 ${mode === 'signup'
                            ? 'bg-blue-600 border-blue-800 text-white'
                            : 'bg-gray-300 border-gray-500 text-gray-700'
                        }`}
                >
                    新規登録
                </button>
            </div>

            <h2 className="text-xl font-bold text-center mb-4">
                {mode === 'login' ? 'メールでログイン' : '新規アカウント作成'}
            </h2>

            {error && (
                <div className="bg-red-600 border-2 border-red-800 text-white p-2 text-center">
                    {error}
                </div>
            )}

            <div>
                <Input
                    type="email"
                    placeholder="メールアドレス"
                    value={email}
                    onChange={setEmail}
                    required
                />
            </div>

            <div>
                <Input
                    type="password"
                    placeholder="パスワード"
                    value={password}
                    onChange={setPassword}
                    required
                    minLength={6}
                />
                {mode === 'signup' && (
                    <p className="text-sm text-gray-600 mt-1">
                        ※ パスワードは6文字以上で入力してください
                    </p>
                )}
            </div>

            <div className="space-y-2">
                <Button type="submit" variant="primary" fullWidth disabled={isSubmitting}>
                    {isSubmitting
                        ? mode === 'login'
                            ? 'ログイン中...'
                            : '登録中...'
                        : mode === 'login'
                            ? 'ログイン'
                            : '新規登録'}
                </Button>
                <Button type="button" variant="secondary" fullWidth onClick={onCancel}>
                    キャンセル
                </Button>
            </div>
        </form>
    );
}
