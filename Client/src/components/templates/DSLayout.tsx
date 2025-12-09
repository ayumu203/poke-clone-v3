interface DSLayoutProps {
    children?: React.ReactNode;
}

/**
 * DS風の全体レイアウトコンポーネント
 * - 外側の背景: 黒色
 * - メイン画面: 400 × 480px、中央配置
 */
export default function DSLayout({ children }: DSLayoutProps) {
    return (
        <div className="min-h-screen bg-black flex items-center justify-center">
            <div className="w-[400px] flex flex-col">
                {children}
            </div>
        </div>
    );
}
