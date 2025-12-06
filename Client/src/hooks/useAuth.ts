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
    const [token, setToken] = useState<string | null>(null);
    const [loading, setLoading] = useState(true);

    // 認証状態の監視
    useEffect(() => {
        const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
            setUser(currentUser);

            if (currentUser) {
                // IDトークンを取得
                const idToken = await currentUser.getIdToken();
                setToken(idToken);
            } else {
                setToken(null);
            }

            setLoading(false);
        });

        return () => unsubscribe();
    }, []);

    // アクセストークン取得
    const getAccessToken = useCallback(async (): Promise<string | null> => {
        if (!user) return null;

        try {
            // 最新のトークンを取得（必要に応じて自動更新）
            const idToken = await user.getIdToken(true);
            return idToken;
        } catch (error) {
            console.error('Failed to get access token:', error);
            return null;
        }
    }, [user]);

    // メール/パスワードでログイン
    const loginWithEmail = useCallback(async (email: string, password: string) => {
        try {
            const userCredential = await signInWithEmailAndPassword(auth, email, password);
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
            setToken(null);
        } catch (error) {
            console.error('Logout failed:', error);
            throw error;
        }
    }, []);

    return {
        user,
        token,
        loading,
        isAuthenticated: !!user,
        getAccessToken,
        loginWithEmail,
        signUpWithEmail,
        loginWithGoogle,
        loginAnonymously,
        logout,
    };
}
