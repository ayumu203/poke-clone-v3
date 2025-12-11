'use client';

import { FirebaseError } from 'firebase/app';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import { FaSignOutAlt } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import HomeButton from '@/components/atoms/HomeButton';
import LoginBottomScreen from '@/components/organisms/LoginBottomScreen';
import StatusBar from '@/components/molecules/StatusBar';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';

export default function LoginPage() {
    const { loginWithEmail, signUpWithEmail, loginWithGoogle, logout, isAuthenticated, loading } = useAuth();
    const router = useRouter();
    const [error, setError] = useState('');
    const [isLoggingOut, setIsLoggingOut] = useState(false);

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
            console.log('Googleログイン成功！');
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
            console.log('ログイン成功！');
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
            console.log('新規登録成功！');
            router.push('/');
        } catch (err) {
            setError(getErrorMessage(err));
            console.error(err);
        }
    };

    const handleLogout = async () => {
        setIsLoggingOut(true);
        try {
            await logout();
            console.log('ログアウト成功！');
        } catch (err) {
            console.error('ログアウトエラー:', err);
        } finally {
            setIsLoggingOut(false);
        }
    };

    // ローディング中
    if (loading) {
        return (
            <DSLayout>
                <TopScreen backgroundImage="/Images/art_011.png" />
                <div className="w-full aspect-[5/3] flex flex-col">
                    <StatusBar />
                    <div className="flex-1 flex items-center justify-center bg-white">
                        <p className="text-xl font-bold">読み込み中...</p>
                    </div>
                </div>
            </DSLayout>
        );
    }

    // ログイン済みの場合はログアウトボタンのみ表示
    if (isAuthenticated) {
        return (
            <DSLayout>
                <TopScreen backgroundImage="/Images/art_011.png" />
                <div className="w-full aspect-[5/3] flex flex-col">
                    <StatusBar />
                    <div
                        className="flex-1 flex items-center justify-center"
                        style={{
                            backgroundImage: `
                                linear-gradient(to right, #e0e0e0 1px, transparent 1px),
                                linear-gradient(to bottom, #e0e0e0 1px, transparent 1px)
                            `,
                            backgroundSize: '20px 20px',
                            backgroundColor: '#fafafa'
                        }}
                    >
                        <div className="w-full max-w-md px-6 space-y-4">
                            <p className="text-xl font-bold text-gray-800 text-center">既にログインしています</p>
                            <Button
                                onClick={handleLogout}
                                variant="secondary"
                                fullWidth
                                disabled={isLoggingOut}
                                icon={<FaSignOutAlt />}
                            >
                                {isLoggingOut ? 'ログアウト中...' : 'ログアウト'}
                            </Button>
                        </div>
                    </div>
                    <div className="bg-gray-800 p-2 flex justify-center gap-4">
                        <HomeButton />
                        <Button
                            onClick={handleLogout}
                            variant="secondary"
                            disabled={isLoggingOut}
                            icon={<FaSignOutAlt />}
                        >
                            {isLoggingOut ? 'ログアウト中' : 'ログアウト'}
                        </Button>
                    </div>
                </div>
            </DSLayout>
        );
    }

    return (
        <DSLayout>
            <TopScreen backgroundImage="/Images/art_011.png" />
            <LoginBottomScreen
                onGoogleLogin={handleGoogleLogin}
                onEmailLogin={handleEmailLogin}
                onEmailSignUp={handleEmailSignUp}
                error={error}
            />
        </DSLayout>
    );
}
