import HomeButton from '@/components/atoms/HomeButton';

interface BottomScreenProps {
    children?: React.ReactNode;
}

/**
 * 下画面コンポーネント
 * - アスペクト比: 5:3 (横:縦)
 * - 最大幅: 400px、レスポンシブ対応
 * - 背景色: 白
 * - メニューやバトルコマンド表示用
 * - HOMEボタンをフッターに配置
 */
export default function BottomScreen({ children }: BottomScreenProps) {
    return (
        <div className="w-full aspect-[5/3] bg-white flex flex-col">
            <div className="flex-1">
                {children}
            </div>
            <div className="bg-gray-800 p-2 flex justify-center">
                <HomeButton />
            </div>
        </div>
    );
}
