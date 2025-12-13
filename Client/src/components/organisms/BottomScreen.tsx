'use client';

import { type ReactNode, useState } from 'react';
import { FaSignOutAlt } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import HomeButton from '@/components/atoms/HomeButton';
import StatusBar from '@/components/molecules/StatusBar';
import { useAuth } from '@/hooks/useAuth';
import { useCommonTexts } from '@/hooks/usePageConfig';

interface BottomScreenProps {
    children?: ReactNode;
    customFooter?: ReactNode;
    backgroundImage?: string;
    darkGrid?: boolean;
}

/**
 * DS風の下画面コンポーネント
 * - StatusBarを常に表示
 * - アスペクト比: 5:3（横:縦）
 * - 格子模様の背景
 * - フッター: HOME + Logout（ログイン時）
 */
export default function BottomScreen({ children, customFooter, backgroundImage, darkGrid }: BottomScreenProps) {
    const { isAuthenticated, logout } = useAuth();
    const [isLoggingOut, setIsLoggingOut] = useState(false);
    const commonTexts = useCommonTexts();

    const handleLogout = async () => {
        setIsLoggingOut(true);
        try {
            await logout();
        } finally {
            setIsLoggingOut(false);
        }
    };

    const defaultFooter = (
        <>
            <HomeButton />
            {isAuthenticated && (
                <Button
                    onClick={handleLogout}
                    variant="secondary"
                    disabled={isLoggingOut}
                    icon={<FaSignOutAlt />}
                >
                    {isLoggingOut ? commonTexts.loggingOut : commonTexts.logout}
                </Button>
            )}
        </>
    );

    // 背景スタイルの決定
    const getBackgroundStyle = () => {
        if (backgroundImage) {
            return {
                backgroundImage: `url(${backgroundImage})`,
                backgroundSize: 'cover',
                backgroundPosition: 'center',
                backgroundRepeat: 'no-repeat',
            };
        }
        if (darkGrid) {
            // 濃い青背景 + 格子模様
            return {
                backgroundColor: '#1a2a4a',
                backgroundImage: `
                    linear-gradient(to right, rgba(100, 180, 200, 0.3) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(100, 180, 200, 0.3) 1px, transparent 1px),
                    linear-gradient(to right, rgba(100, 180, 200, 0.15) 1px, transparent 1px),
                    linear-gradient(to bottom, rgba(100, 180, 200, 0.15) 1px, transparent 1px)
                `,
                backgroundSize: '40px 40px, 40px 40px, 10px 10px, 10px 10px',
            };
        }
        // デフォルト: 明るい格子模様
        return {
            backgroundImage: `
                linear-gradient(to right, #e0e0e0 1px, transparent 1px),
                linear-gradient(to bottom, #e0e0e0 1px, transparent 1px)
            `,
            backgroundSize: '20px 20px',
            backgroundColor: '#fafafa',
        };
    };

    return (
        <div className="w-full aspect-[5/3] bg-white flex flex-col">
            <StatusBar />
            <div
                className="flex-1 overflow-auto"
                style={getBackgroundStyle()}
            >
                {children}
            </div>
            <div className="bg-gray-800 p-2 flex justify-center gap-4">
                {customFooter || defaultFooter}
            </div>
        </div>
    );
}
