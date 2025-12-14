/**
 * セッション管理サービスクラス
 * HTTPOnly Cookieを使用したセッション管理
 */
export class SessionService {
  /**
   * セッションCookieを作成
   * @param token Firebase IDトークン
   */
  async createSession(token: string): Promise<void> {
    await fetch("/api/auth/session", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ token }),
    });
  }

  /**
   * セッションCookieを削除
   */
  async destroySession(): Promise<void> {
    await fetch("/api/auth/logout", { method: "POST" });
  }

  /**
   * セッションの有効性を検証
   */
  async verifySession(): Promise<{ valid: boolean; uid?: string; email?: string }> {
    const response = await fetch("/api/auth/verify");
    return response.json();
  }
}

// シングルトンインスタンス
export const sessionService = new SessionService();
