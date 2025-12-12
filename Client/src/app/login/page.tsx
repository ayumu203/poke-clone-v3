'use client';

import { FirebaseError } from 'firebase/app';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

import BottomScreen from '@/components/organisms/BottomScreen';
import LoginBottomScreen from '@/components/organisms/LoginBottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';
import { usePageConfig } from '@/hooks/usePageConfig';

export default function LoginPage() {
    const { loginWithEmail, signUpWithEmail, loginWithGoogle, isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const pageConfig = usePageConfig('login');
    const [error, setError] = useState('');

    const getErrorMessage = (error: unknown): string => {
        if (error instanceof FirebaseError) {
            switch (error.code) {
                case 'auth/invalid-credential':
                    return 'メールアドレスまたはパスワードが間違っています';
                case 'auth/email-already-in-use':
                    return 'このメールアドレスは既に登録されています';
                case 'auth/weak-password':
                    return 'パスワードは6文字以上である必要があります';
                case 'auth/invalid-email':
                    return '有効なメールアドレスを入力してください';
                default:
                    return `エラーが発生しました: ${error.code}`;
            }
        }
        return '予期しないエラーが発生しました';
    };

    const handleGoogleLogin = async () => {
        setError('');
        try {
            await loginWithGoogle();
            router.push('/');
        } catch (err) {
            setError(getErrorMessage(err));
            console.error(err);
        }
    };

    const handleEmailLogin = async (email: string, password: string) => {
        setError('');
        try {
            await loginWithEmail(email, password);
            router.push('/');
        } catch (err) {
            setError(getErrorMessage(err));
            console.error(err);
        }
    };

    const handleEmailSignUp = async (email: string, password: string) => {
        setError('');
        try {
            await signUpWithEmail(email, password);
            router.push('/');
        } catch (err) {
            setError(getErrorMessage(err));
            console.error(err);
        }
    };

    // ローディング中
    if (loading) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen>
                    <div className="flex items-center justify-center h-full bg-white">
                        <p className="text-xl font-bold">読み込み中...</p>
                    </div>
                </BottomScreen>
            </DSLayout>
        );
    }

    if (isAuthenticated) {
        return (
            <DSLayout bgmSrc={pageConfig.bgm}>
                <TopScreen backgroundImage={pageConfig.image} />
                <BottomScreen>
                    <div className="h-full flex items-center justify-center">
                        <p className="text-xl font-bold text-gray-800">既にログインしています</p>
                    </div>
                </BottomScreen>
            </DSLayout>
        );
    }

    return (
        <DSLayout bgmSrc={pageConfig.bgm}>
            <TopScreen backgroundImage={pageConfig.image} />
            <LoginBottomScreen
                onGoogleLogin={handleGoogleLogin}
                onEmailLogin={handleEmailLogin}
                onEmailSignUp={handleEmailSignUp}
                error={error}
            />
        </DSLayout>
    );
}
