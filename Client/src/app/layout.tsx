import "./globals.css";

import type { Metadata } from "next";
import { DotGothic16 } from 'next/font/google';

import { Providers } from '@/components/providers/Providers';

const dotGothic16 = DotGothic16({
  weight: '400',
  subsets: ['latin'],
  display: 'swap',
});

export const metadata: Metadata = {
  title: "Poke-Clone-V3",
  description: "リアルタイムなポケモン対戦を再現しよう！",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="ja">
      <body className={dotGothic16.className}>
        <Providers>
          {children}
        </Providers>
      </body>
    </html>
  );
}
