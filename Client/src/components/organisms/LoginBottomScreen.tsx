'use client';

import { useState } from 'react';
import { FaEnvelope } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import EmailLoginForm from '@/components/molecules/EmailLoginForm';
import StatusBar from '@/components/molecules/StatusBar';

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
                <StatusBar />
                <div
                    className="flex-1 overflow-auto"
                    style={{
                        backgroundImage: `
                            linear-gradient(to right, #e0e0e0 1px, transparent 1px),
                            linear-gradient(to bottom, #e0e0e0 1px, transparent 1px)
                        `,
                        backgroundSize: '20px 20px',
                        backgroundColor: '#fafafa'
                    }}
                >
                    <EmailLoginForm
                        onLogin={onEmailLogin}
                        onSignUp={onEmailSignUp}
                        onCancel={() => setShowEmailForm(false)}
                        error={error}
                    />
                </div>
            </div>
        );
    }

    return (
        <div className="w-full aspect-[5/3] flex flex-col">
            <StatusBar />
            <div
                className="flex-1 p-6 space-y-4 flex flex-col items-center justify-center"
                style={{
                    backgroundImage: `
                        linear-gradient(to right, #e0e0e0 1px, transparent 1px),
                        linear-gradient(to bottom, #e0e0e0 1px, transparent 1px)
                    `,
                    backgroundSize: '20px 20px',
                    backgroundColor: '#fafafa'
                }}
            >
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
        </div>
    );
}
