'use client';

import { type ReactNode, useState } from 'react';
import { FaSignOutAlt } from 'react-icons/fa';

import Button from '@/components/atoms/Button';
import HomeButton from '@/components/atoms/HomeButton';
import StatusBar from '@/components/molecules/StatusBar';
import { useAuth } from '@/hooks/useAuth';

interface BottomScreenProps {
    children?: ReactNode;
    customFooter?: ReactNode;
}

/**
 * DS風の下画面コンポーネント
 * - StatusBarを常に表示
 * - アスペクト比: 5:3（横:縦）
 * - 格子模様の背景
 * - フッター: HOME + Logout（ログイン時）
 */
export default function BottomScreen({ children, customFooter }: BottomScreenProps) {
    const { isAuthenticated, logout } = useAuth();
    const [isLoggingOut, setIsLoggingOut] = useState(false);

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
                    {isLoggingOut ? 'ログアウト中' : 'ログアウト'}
                </Button>
            )}
        </>
    );

    return (
        <div className="w-full aspect-[5/3] bg-white flex flex-col">
            <StatusBar />
            <div
                className="flex-1 overflow-auto"
                style={{
                    backgroundImage: `
                        linear-gradient(to right, #e0e0e0 1px, transparent 1px),
                        linear-gradient(to bottom, #e0e0e0 1px, transparent 1px)
                    `,
                    backgroundSize: '20px 20px',
                    backgroundColor: '#fafafa'
                }}
            >
                {children}
            </div>
            <div className="bg-gray-800 p-2 flex justify-center gap-4">
                {customFooter || defaultFooter}
            </div>
        </div>
    );
}
