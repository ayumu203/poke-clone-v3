'use client';

import { FirebaseError } from 'firebase/app';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

import LoginBottomScreen from '@/components/organisms/LoginBottomScreen';
import TopScreen from '@/components/organisms/TopScreen';
import DSLayout from '@/components/templates/DSLayout';
import { useAuth } from '@/hooks/useAuth';

export default function LoginPage() {
    const { loginWithEmail, signUpWithEmail, loginWithGoogle } = useAuth();
    const router = useRouter();
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
