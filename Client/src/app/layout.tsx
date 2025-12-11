'use client';

import "./globals.css";

import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import type { Metadata } from "next";
import { DotGothic16 } from 'next/font/google';
import { useState } from 'react';

import { BGMProvider } from '@/contexts/BGMContext';

const dotGothic16 = DotGothic16({
  weight: '400',
  subsets: ['latin'],
  display: 'swap',
});

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
      <body className={dotGothic16.className}>
        <QueryClientProvider client={queryClient}>
          <BGMProvider>
            {children}
          </BGMProvider>
        </QueryClientProvider>
      </body>
    </html>
  );
}

