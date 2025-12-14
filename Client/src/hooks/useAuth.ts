import type { User } from "firebase/auth";
import { onAuthStateChanged } from "firebase/auth";
import { useCallback, useEffect, useState } from "react";

import { authService } from "@/lib/auth/authService";
import { sessionService } from "@/lib/auth/sessionService";
import { auth } from "@/lib/firebase/config";

const DEFAULT_PLAYER_NAME = "プレイヤー";
const DEFAULT_ICON_URL =
  "https://www.project-voltage.jp/assets/img/dl_contents/thumbnail/icon/high/icon_denki.jpg";

export function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  // 認証状態の監視
  useEffect(() => {
    const unsubscribe = onAuthStateChanged(auth, async (currentUser) => {
      setUser(currentUser);

      if (currentUser) {
        try {
          // IDトークンを取得してセッションCookieに保存
          const idToken = await currentUser.getIdToken();
          await sessionService.createSession(idToken);

          // バックエンドAPI URLを取得
          const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

          // プレイヤー情報の存在確認
          const checkResponse = await fetch(`${apiUrl}/api/Player/me`, {
            headers: {
              Authorization: `Bearer ${idToken}`,
            },
          });

          // プレイヤー情報が存在しない場合（404）、自動作成
          if (checkResponse.status === 404) {
            if (process.env.NODE_ENV === "development") {
              // eslint-disable-next-line no-console
              console.log("プレイヤーアカウントを作成します...");
            }
            const createResponse = await fetch(`${apiUrl}/api/Player/me`, {
              method: "POST",
              headers: {
                Authorization: `Bearer ${idToken}`,
                "Content-Type": "application/json",
              },
              body: JSON.stringify({
                name: DEFAULT_PLAYER_NAME,
                iconUrl: DEFAULT_ICON_URL,
              }),
            });

            if (createResponse.ok) {
              if (process.env.NODE_ENV === "development") {
                // eslint-disable-next-line no-console
                console.log("プレイヤーアカウントを作成しました");
              }
            } else {
              const errorText = await createResponse.text();
              if (process.env.NODE_ENV === "development") {
                 
                console.error(
                  "プレイヤーアカウントの作成に失敗しました:",
                  createResponse.status,
                  errorText
                );
              }
            }
          }
        } catch (error) {
          if (process.env.NODE_ENV === "development") {
             
            console.error("プレイヤーアカウント処理でエラーが発生しました:", error);
          }
        }
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
      console.error("Login failed:", error);
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
      console.error("Sign up failed:", error);
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
      console.error("Google login failed:", error);
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
      console.error("Anonymous login failed:", error);
      throw error;
    }
  }, []);

  // ログアウト
  const logout = useCallback(async () => {
    try {
      await authService.logout();
      await sessionService.destroySession();
    } catch (error) {
      console.error("Logout failed:", error);
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
