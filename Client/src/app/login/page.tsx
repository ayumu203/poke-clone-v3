'use client';

import DSLayout from '@/components/templates/DSLayout';
import TopScreen from '@/components/organisms/TopScreen';
import LoginBottomScreen from '@/components/organisms/LoginBottomScreen';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

export default function LoginPage() {
    const { loginWithEmail, loginWithGoogle } = useAuth();
    const router = useRouter();
    const [error, setError] = useState('');

    const handleGoogleLogin = async () => {
        setError('');
        try {
            const user = await loginWithGoogle();
            const token = await user.getIdToken();
            console.log('Googleログイン成功！');
            console.log('Firebase ID Token:', token);
            (window as any).token = token;
            router.push('/');
        } catch (err) {
            setError('Googleログインに失敗しました');
            console.error(err);
        }
    };

    const handleEmailLogin = async (email: string, password: string) => {
        setError('');
        try {
            const user = await loginWithEmail(email, password);
            const token = await user.getIdToken();
            console.log('ログイン成功！');
            console.log('Firebase ID Token:', token);
            (window as any).token = token;
            router.push('/');
        } catch (err) {
            setError('ログインに失敗しました');
            console.error(err);
        }
    };

    return (
        <DSLayout>
            <TopScreen backgroundImage="/Images/art_011.png" />
            <LoginBottomScreen
                onGoogleLogin={handleGoogleLogin}
                onEmailLogin={handleEmailLogin}
                error={error}
            />
        </DSLayout>
    );
}
