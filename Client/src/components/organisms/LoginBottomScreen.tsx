'use client';

import { useState } from 'react';
import { FaEnvelope } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import HomeButton from '@/components/atoms/HomeButton';
import EmailLoginForm from '@/components/molecules/EmailLoginForm';

interface LoginBottomScreenProps {
    onGoogleLogin: () => Promise<void>;
    onEmailLogin: (email: string, password: string) => Promise<void>;
    onEmailSignUp: (email: string, password: string) => Promise<void>;
    error?: string;
}

/**
 * ログインページの下画面コンポーネント
 * - アスペクト比: 5:3 (横:縦)、レスポンシブ対応
 * - ステータスバー: 時刻、ネットワーク名、アイコン
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
            <div className="w-full aspect-[5/3] flex flex-col">
                <div className="flex-1 overflow-auto">
                    <EmailLoginForm
                        onLogin={onEmailLogin}
                        onSignUp={onEmailSignUp}
                        onCancel={() => setShowEmailForm(false)}
                        error={error}
                    />
                </div>
                <div className="bg-gray-800 p-2 flex justify-center">
                    <HomeButton />
                </div>
            </div>
        );
    }

    return (
        <div className="w-full aspect-[5/3] flex flex-col">
            <div className="h-full p-6 space-y-4 flex flex-col justify-center items-center">
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
                    icon={<img src="/icon/google-logo.png" alt="Google" className="w-5 h-5" />}
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
            <div className="bg-gray-800 p-2 flex justify-center">
                <HomeButton />
            </div>
        </div>
    );
}
