import type { User } from "firebase/auth";
import {
  createUserWithEmailAndPassword,
  GoogleAuthProvider,
  signInAnonymously,
  signInWithEmailAndPassword,
  signInWithPopup,
  signOut,
} from "firebase/auth";

import { auth } from "@/lib/firebase/config";

/**
 * Firebase Authentication サービスクラス
 * 認証ロジックを一元管理
 */
export class AuthService {
  /**
   * メールアドレスとパスワードでログイン
   */
  async loginWithEmail(email: string, password: string): Promise<User> {
    const userCredential = await signInWithEmailAndPassword(auth, email, password);
    return userCredential.user;
  }

  /**
   * メールアドレスとパスワードでサインアップ
   */
  async signUpWithEmail(email: string, password: string): Promise<User> {
    const userCredential = await createUserWithEmailAndPassword(auth, email, password);
    return userCredential.user;
  }

  /**
   * Googleアカウントでログイン
   */
  async loginWithGoogle(): Promise<User> {
    const provider = new GoogleAuthProvider();
    const userCredential = await signInWithPopup(auth, provider);
    return userCredential.user;
  }

  /**
   * 匿名ログイン（開発・テスト用）
   */
  async loginAnonymously(): Promise<User> {
    const userCredential = await signInAnonymously(auth);
    return userCredential.user;
  }

  /**
   * ログアウト
   */
  async logout(): Promise<void> {
    await signOut(auth);
  }
}

// シングルトンインスタンス
export const authService = new AuthService();
