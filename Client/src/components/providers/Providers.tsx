'use client';

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactNode, useState } from 'react';

import { BGMProvider } from '@/contexts/BGMContext';

/**
 * アプリケーション全体のプロバイダーをまとめたコンポーネント
 * Client Componentとして実装
 */
export function Providers({ children }: { children: ReactNode }) {
    const [queryClient] = useState(
        () =>
            new QueryClient({
                defaultOptions: {
                    queries: {
                        staleTime: 5 * 60 * 1000, // 5分
                        gcTime: 10 * 60 * 1000, // 10分 (旧cacheTime)
                        retry: 1,
                        refetchOnWindowFocus: false, // バトル中の誤リフェッチを防ぐ
                    },
                },
            })
    );

    return (
        <QueryClientProvider client={queryClient}>
            <BGMProvider>{children}</BGMProvider>
        </QueryClientProvider>
    );
}
