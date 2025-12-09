interface BottomScreenProps {
    children?: React.ReactNode;
}

/**
 * 下画面コンポーネント
 * - サイズ: 400 × 240px
 * - 背景色: 白
 * - メニューやバトルコマンド表示用
 */
export default function BottomScreen({ children }: BottomScreenProps) {
    return (
        <div className="w-[400px] h-[240px] bg-white">
            {children}
        </div>
    );
}
