'use client';

import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';
import { useState } from 'react';

export default function LoginPage() {
    const { loginWithEmail, loginWithGoogle, loginAnonymously } = useAuth();
    const router = useRouter();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');

    const handleEmailLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        try {
            const user = await loginWithEmail(email, password);
            const token = await user.getIdToken();
            console.log('ログイン成功！');
            console.log('Firebase ID Token:', token);
            console.log('コピーするには: copy(token) を実行');
            (window as any).token = token;
            router.push('/');
        } catch (err) {
            setError('ログインに失敗しました');
            console.error(err);
        }
    };

    const handleGoogleLogin = async () => {
        setError('');
        try {
            const user = await loginWithGoogle();
            const token = await user.getIdToken();
            console.log(' ログイン成功！');
            console.log('Firebase ID Token:', token);
            console.log('コピーするには: copy(token) を実行');
            (window as any).token = token;
            router.push('/');
        } catch (err) {
            setError('Googleログインに失敗しました');
            console.error(err);
        }
    };

    const handleAnonymousLogin = async () => {
        setError('');
        try {
            const user = await loginAnonymously();
            const token = await user.getIdToken();
            console.log('ログイン成功！');
            console.log('Firebase ID Token:', token);
            console.log('コピーするには: copy(token) を実行');
            (window as any).token = token;
            router.push('/');
        } catch (err) {
            setError('ゲストログインに失敗しました');
            console.error(err);
        }
    };

    return (
        <div style={{ maxWidth: '400px', margin: '50px auto', padding: '20px' }}>
            <h1>ログイン</h1>

            {error && (
                <div style={{ color: 'red', marginBottom: '20px' }}>
                    {error}
                </div>
            )}

            <form onSubmit={handleEmailLogin} style={{ marginBottom: '20px' }}>
                <div style={{ marginBottom: '10px' }}>
                    <input
                        type="email"
                        placeholder="メール"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        style={{ width: '100%', padding: '8px' }}
                    />
                </div>
                <div style={{ marginBottom: '10px' }}>
                    <input
                        type="password"
                        placeholder="パスワード"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        style={{ width: '100%', padding: '8px' }}
                    />
                </div>
                <button type="submit" style={{ width: '100%', padding: '10px' }}>
                    メールでログイン
                </button>
            </form>

            <button
                onClick={handleGoogleLogin}
                style={{
                    width: '100%',
                    padding: '10px',
                    marginBottom: '10px',
                    backgroundColor: '#4285f4',
                    color: 'white',
                    border: 'none',
                    cursor: 'pointer'
                }}
            >
                Googleでログイン
            </button>

            <button
                onClick={handleAnonymousLogin}
                style={{
                    width: '100%',
                    padding: '10px',
                    backgroundColor: '#666',
                    color: 'white',
                    border: 'none',
                    cursor: 'pointer'
                }}
            >
                ゲストとしてプレイ
            </button>
        </div>
    );
}
