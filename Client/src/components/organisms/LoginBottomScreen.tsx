'use client';

import { useState } from 'react';
import { FaGoogle, FaEnvelope } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import EmailLoginForm from '@/components/molecules/EmailLoginForm';

interface LoginBottomScreenProps {
    onGoogleLogin: () => Promise<void>;
    onEmailLogin: (email: string, password: string) => Promise<void>;
    onEmailSignUp: (email: string, password: string) => Promise<void>;
    error?: string;
}

/**
 * ログインページの下画面コンポーネント
 * - 初期状態: Googleログインボタン、Emailログインボタン
 * - Emailログインボタンクリック後: EmailLoginFormを表示
 */
export default function LoginBottomScreen({
    onGoogleLogin,
    onEmailLogin,
    onEmailSignUp,
    error,
}: LoginBottomScreenProps) {
    const [showEmailForm, setShowEmailForm] = useState(false);
    const [isGoogleLoading, setIsGoogleLoading] = useState(false);

    const handleGoogleLogin = async () => {
        setIsGoogleLoading(true);
        try {
            await onGoogleLogin();
        } finally {
            setIsGoogleLoading(false);
        }
    };

    if (showEmailForm) {
        return (
            <EmailLoginForm
                onLogin={onEmailLogin}
                onSignUp={onEmailSignUp}
                onCancel={() => setShowEmailForm(false)}
                error={error}
            />
        );
    }

    return (
        <div className="p-6 space-y-4 flex flex-col items-center justify-center h-full">
            <h1 className="text-2xl font-bold text-center mb-4">ログイン</h1>

            {error && (
                <div className="bg-red-600 border-2 border-red-800 text-white p-2 text-center w-full">
                    {error}
                </div>
            )}

            <Button
                onClick={handleGoogleLogin}
                variant="google"
                fullWidth
                disabled={isGoogleLoading}
                icon={<FaGoogle />}
            >
                {isGoogleLoading ? 'ログイン中...' : 'Googleでログイン'}
            </Button>

            <Button
                onClick={() => setShowEmailForm(true)}
                variant="primary"
                fullWidth
                icon={<FaEnvelope />}
            >
                メールでログイン
            </Button>
        </div>
    );
}
