'use client';

import Image from 'next/image';
import { useRouter } from 'next/navigation';
import { type ReactNode } from 'react';
import { type IconType } from 'react-icons';

interface MenuButtonProps {
    icon?: IconType;
    imageSrc?: string;
    href: string;
    children: ReactNode;
}

/**
 * メニュー用ボタンコンポーネント
 * - 背景色: #333333
 * - 横幅小さめ、縦広め
 * - アイコンまたは画像 + テキスト（横並び）
 */
export default function MenuButton({ icon: Icon, imageSrc, href, children }: MenuButtonProps) {
    const router = useRouter();

    const handleClick = () => {
        router.push(href);
    };

    return (
        <button
            onClick={handleClick}
            className="relative w-full max-w-xs py-3 px-4 sm:py-4 sm:px-6 border-2 border-gray-600 text-white font-bold hover:bg-gray-700 transition-colors flex items-center justify-center"
            style={{ backgroundColor: '#333333' }}
        >
            {Icon && (
                <div className="absolute left-[2px] sm:left-4 flex items-center">
                    <Icon className="text-2xl sm:text-4xl" />
                </div>
            )}
            {imageSrc && (
                <div className="absolute left-[2px] sm:left-4 w-8 h-8 sm:w-12 sm:h-12">
                    <Image
                        src={imageSrc}
                        alt=""
                        fill
                        className="object-contain"
                    />
                </div>
            )}
            <span className="text-[16px] sm:text-[25px] leading-tight">{children}</span>
        </button>
    );
}
