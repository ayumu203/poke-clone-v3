'use client';

import "./globals.css";

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { Metadata } from "next";
import { useState } from 'react';

const metadata: Metadata = {
  title: "Poke-Clone-V3",
  description: "リアルタイムなポケモン対戦を再現しよう！",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const [queryClient] = useState(() => new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: 60 * 1000, // デフォルトで1分間キャッシュ
        retry: 1, // 失敗時1回のみリトライ
      },
    },
  }));

  return (
    <html lang="ja">
      <head>
        <title>{metadata.title as string}</title>
        <meta name="description" content={metadata.description as string} />
      </head>
      <body>
        <QueryClientProvider client={queryClient}>
          {children}
        </QueryClientProvider>
      </body>
    </html>
  );
}

