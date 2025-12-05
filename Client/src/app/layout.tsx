import type { Metadata } from "next";
import "./globals.css";



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
    <html lang="en">
      <body
      >
        {children}
      </body>
    </html>
  );
}
