import Header from "@/components/organisms/Header";

interface DSLayoutProps {
  children?: React.ReactNode;
  bgmSrc?: string;
}

/**
 * DS風の全体レイアウトコンポーネント
 * - 外側の背景: 黒色
 * - ヘッダー: 外部リンク、BGMトグル
 * - メイン画面: 最大600px、レスポンシブ対応
 * - 各画面: 5:3のアスペクト比（横:縦）
 */
export default function DSLayout({ children, bgmSrc }: DSLayoutProps) {
  return (
    <div className="flex min-h-screen items-center justify-center bg-black p-4">
      <div className="flex w-full max-w-[800px] flex-col">
        <Header bgmSrc={bgmSrc} />
        {children}
      </div>
    </div>
  );
}
