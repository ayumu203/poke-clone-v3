'use client';

import Image from 'next/image';
import { useState } from 'react';
import { FaEnvelope } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import BottomScreen from '@/components/organisms/BottomScreen';
import EmailLoginForm from '@/components/molecules/EmailLoginForm';
import { usePageConfig } from '@/hooks/usePageConfig';

interface LoginBottomScreenProps {
    onGoogleLogin: () => Promise<void>;
    onEmailLogin: (email: string, password: string) => Promise<void>;
    onEmailSignUp: (email: string, password: string) => Promise<void>;
    error?: string;
}

/**
 * ログインページの下画面コンポーネント
 * - BottomScreenでラップし、StatusBarと背景画像を表示
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
    const pageConfig = usePageConfig('login');
    const { t } = pageConfig;

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
            <BottomScreen>
                <EmailLoginForm
                    onLogin={onEmailLogin}
                    onSignUp={onEmailSignUp}
                    onCancel={() => setShowEmailForm(false)}
                    error={error}
                    t={t}
                />
            </BottomScreen>
        );
    }

    return (
        <BottomScreen>
            <div className="h-full p-6 space-y-4 flex flex-col justify-center items-center">
                {error && (
                    <div className="bg-red-600 border-2 border-red-800 text-white p-2 text-center w-full">
                        {error}
                    </div>
                )}

                <p className="text-gray-700 mb-10 font-bold text-center text-3xl">
                    {t.accountRequired}
                </p>

                <Button
                    onClick={handleGoogleLogin}
                    variant="google"
                    fullWidth
                    disabled={isGoogleLoading}
                    icon={
                        <Image
                            src="/icon/google-logo.png"
                            alt="Google"
                            width={20}
                            height={20}
                        />
                    }
                >
                    {isGoogleLoading ? t.googleLoading : t.googleLogin}
                </Button>

                <Button
                    onClick={() => setShowEmailForm(true)}
                    variant="primary"
                    fullWidth
                    icon={<FaEnvelope />}
                >
                    {t.emailLogin}
                </Button>
            </div>
        </BottomScreen>
    );
}
