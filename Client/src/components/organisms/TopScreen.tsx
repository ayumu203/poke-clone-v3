import Image from 'next/image';

interface TopScreenProps {
    backgroundImage?: string;
    children?: React.ReactNode;
}

/**
 * 上画面コンポーネント
 * - アスペクト比: 5:3 (横:縦)
 * - 最大幅: 400px、レスポンシブ対応
 * - 背景画像表示（デフォルト: art_003.png）
 * - object-fit: cover ではみ出た部分をクロップ
 */
export default function TopScreen({
    backgroundImage = '/Images/art_003.png',
    children,
}: TopScreenProps) {
    return (
        <div className="w-full aspect-[5/3] relative overflow-hidden">
            <Image
                src={backgroundImage}
                alt="背景画像"
                fill
                sizes="(max-width: 800px) 100vw, 800px"
                className="object-cover"
                priority
            />
            {children && (
                <div className="absolute inset-0 z-10">{children}</div>
            )}
        </div>
    );
}
