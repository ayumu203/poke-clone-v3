interface DSLayoutProps {
    children?: React.ReactNode;
}

/**
 * DS風の全体レイアウトコンポーネント
 * - 外側の背景: 黒色
 * - メイン画面: 最大400px、レスポンシブ対応
 * - 各画面: 5:3のアスペクト比（横:縦）
 */
export default function DSLayout({ children }: DSLayoutProps) {
    return (
        <div className="min-h-screen bg-black flex items-center justify-center p-4">
            <div className="w-full max-w-[400px] flex flex-col">
                {children}
            </div>
        </div>
    );
}
