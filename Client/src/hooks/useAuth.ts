import { useCallback, useState } from 'react';

/**
 * モック認証フック
 * 本番環境ではAzure AD B2Cを使用しますが、開発環境ではモックトークンを返します
 */
export function useAuth() {
    const [token, setToken] = useState<string | null>('mock-jwt-token');

    const getAccessToken = useCallback(async (): Promise<string | null> => {
        // モック実装: 実際のトークンを返す
        return token;
    }, [token]);

    const login = useCallback(async (_username: string, _password: string) => {
        // モック実装: 常に成功するログイン処理
        const mockToken = `mock-jwt-token-${Date.now()}`;
        setToken(mockToken);
        return mockToken;
    }, []);

    const logout = useCallback(() => {
        setToken(null);
    }, []);

    return {
        token,
        getAccessToken,
        login,
        logout,
        isAuthenticated: token !== null,
    };
}
