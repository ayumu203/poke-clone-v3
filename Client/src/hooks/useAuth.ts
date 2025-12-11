import type { User } from 'firebase/auth';
import { onAuthStateChanged } from 'firebase/auth';
import { useCallback, useEffect, useState } from 'react';

import { authService } from '@/lib/auth/authService';
import { sessionService } from '@/lib/auth/sessionService';
import { auth } from '@/lib/firebase/config';

export function useAuth() {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    // 認証状態の監視
    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
            setUser(currentUser);

            if (currentUser) {
                // IDトークンを取得してセッションCookieに保存
                const idToken = await currentUser.getIdToken();
                await sessionService.createSession(idToken);
            } else {
                // ユーザーがいない場合はセッションを削除
                await sessionService.destroySession();
            }

            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    // メール/パスワードでログイン
    const loginWithEmail = useCallback(async (email: string, password: string) => {
        try {
            const user = await authService.loginWithEmail(email, password);
            const idToken = await user.getIdToken();
            await sessionService.createSession(idToken);
            return user;
        } catch (error) {
            console.error('Login failed:', error);
            throw error;
        }
    }, []);

    // メール/パスワードでサインアップ
    const signUpWithEmail = useCallback(async (email: string, password: string) => {
        try {
            const user = await authService.signUpWithEmail(email, password);
            const idToken = await user.getIdToken();
            await sessionService.createSession(idToken);
            return user;
        } catch (error) {
            console.error('Sign up failed:', error);
            throw error;
        }
    }, []);

    // Googleでログイン
    const loginWithGoogle = useCallback(async () => {
        try {
            const user = await authService.loginWithGoogle();
            const idToken = await user.getIdToken();
            await sessionService.createSession(idToken);
            return user;
        } catch (error) {
            console.error('Google login failed:', error);
            throw error;
        }
    }, []);

    // 匿名ログイン
    const loginAnonymously = useCallback(async () => {
        try {
            const user = await authService.loginAnonymously();
            const idToken = await user.getIdToken();
            await sessionService.createSession(idToken);
            return user;
        } catch (error) {
            console.error('Anonymous login failed:', error);
            throw error;
        }
    }, []);

    // ログアウト
    const logout = useCallback(async () => {
        try {
            await authService.logout();
            await sessionService.destroySession();
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
