import Image from 'next/image';

interface TopScreenProps {
    backgroundImage?: string;
    children?: React.ReactNode;
}

/**
 * 上画面コンポーネント
 * - サイズ: 400 × 240px
 * - 背景画像表示（デフォルト: art_003.png）
 * - object-fit: cover ではみ出た部分をクロップ
 */
export default function TopScreen({
    backgroundImage = '/Images/art_003.png',
    children,
}: TopScreenProps) {
    return (
        <div className="w-[400px] h-[240px] relative overflow-hidden">
            <Image
                src={backgroundImage}
                alt="背景画像"
                fill
                className="object-cover"
                priority
            />
            {children && (
                <div className="absolute inset-0 z-10">{children}</div>
            )}
        </div>
    );
}
