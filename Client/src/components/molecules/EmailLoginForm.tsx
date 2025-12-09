'use client';

import { useState } from 'react';
import Button from '@/components/atoms/Button';
import Input from '@/components/atoms/Input';

interface EmailLoginFormProps {
    onSubmit: (email: string, password: string) => Promise<void>;
    onCancel: () => void;
    error?: string;
}

/**
 * メールログインフォームコンポーネント
 * - Email、Passwordの入力フィールド
 * - バリデーション
 * - エラー表示
 */
export default function EmailLoginForm({ onSubmit, onCancel, error }: EmailLoginFormProps) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setIsSubmitting(true);
        try {
            await onSubmit(email, password);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-4 p-4">
            <h2 className="text-xl font-bold text-center mb-4">メールでログイン</h2>

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
                />
            </div>

            <div className="space-y-2">
                <Button type="submit" variant="primary" fullWidth disabled={isSubmitting}>
                    {isSubmitting ? 'ログイン中...' : 'ログイン'}
                </Button>
                <Button type="button" variant="secondary" fullWidth onClick={onCancel}>
                    キャンセル
                </Button>
            </div>
        </form>
    );
}
