import type { User } from 'firebase/auth';
import {
    createUserWithEmailAndPassword,
    GoogleAuthProvider,
    onAuthStateChanged,
    signInAnonymously,
    signInWithEmailAndPassword,
    signInWithPopup,
    signOut,
} from 'firebase/auth';
import { useCallback, useEffect, useState } from 'react';

import { auth } from '@/lib/firebase/config';

export function useAuth() {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    // 認証状態の監視
    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
            setUser(currentUser);

            if (currentUser) {
                // IDトークンを取得してHTTPOnly Cookieに保存
                const idToken = await currentUser.getIdToken();
                await fetch('/api/auth/session', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ token: idToken }),
                });
            } else {
                // ユーザーがいない場合はセッションを削除
                await fetch('/api/auth/logout', { method: 'POST' });
            }

            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    // メール/パスワードでログイン
    const loginWithEmail = useCallback(async (email: string, password: string) => {
        try {
            const userCredential = await signInWithEmailAndPassword(auth, email, password);
            const idToken = await userCredential.user.getIdToken();

            // セッションCookieに保存
            await fetch('/api/auth/session', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: idToken }),
            });

            return userCredential.user;
        } catch (error) {
            console.error('Login failed:', error);
            throw error;
        }
    }, []);

    // メール/パスワードでサインアップ
    const signUpWithEmail = useCallback(async (email: string, password: string) => {
        try {
            const userCredential = await createUserWithEmailAndPassword(auth, email, password);
            const idToken = await userCredential.user.getIdToken();

            // セッションCookieに保存
            await fetch('/api/auth/session', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: idToken }),
            });

            return userCredential.user;
        } catch (error) {
            console.error('Sign up failed:', error);
            throw error;
        }
    }, []);

    // Googleでログイン
    const loginWithGoogle = useCallback(async () => {
        try {
            const provider = new GoogleAuthProvider();
            const userCredential = await signInWithPopup(auth, provider);
            const idToken = await userCredential.user.getIdToken();

            // セッションCookieに保存
            await fetch('/api/auth/session', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: idToken }),
            });

            return userCredential.user;
        } catch (error) {
            console.error('Google login failed:', error);
            throw error;
        }
    }, []);

    // 匿名ログイン（開発・テスト用）
    const loginAnonymously = useCallback(async () => {
        try {
            const userCredential = await signInAnonymously(auth);
            const idToken = await userCredential.user.getIdToken();

            // セッションCookieに保存
            await fetch('/api/auth/session', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ token: idToken }),
            });

            return userCredential.user;
        } catch (error) {
            console.error('Anonymous login failed:', error);
            throw error;
        }
    }, []);

    // ログアウト
    const logout = useCallback(async () => {
        try {
            await signOut(auth);
            // セッションCookieを削除
            await fetch('/api/auth/logout', { method: 'POST' });
        } catch (error) {
            console.error('Logout failed:', error);
            throw error;
        }
    }, []);

    return {
        user,
        loading,
        isAuthenticated: !!user,
        loginWithEmail,
        signUpWithEmail,
        loginWithGoogle,
        loginAnonymously,
        logout,
    };
}
