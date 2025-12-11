'use client';

import { useRouter } from 'next/navigation';
import { FaHome } from 'react-icons/fa';

interface HomeButtonProps {
    variant?: 'default' | 'compact';
}

/**
 * HOMEボタンコンポーネント
 * - ドット調（ピクセル風）のデザイン
 * - ルートページへ遷移
 */
export default function HomeButton({ variant = 'default' }: HomeButtonProps) {
    const router = useRouter();

    const handleClick = () => {
        router.push('/');
    };

    return (
        <button
            onClick={handleClick}
            className="px-6 py-2 bg-gray-100 border-2 border-gray-400 text-gray-800 font-bold hover:bg-gray-200 transition-colors flex items-center justify-center gap-2"
        >
            <FaHome className="text-lg" />
            <span>HOME</span>
        </button>
    );
}
