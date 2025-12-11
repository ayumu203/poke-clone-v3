'use client';

import { useEffect, useState } from 'react';
import { FaVolumeUp, FaWifi } from 'react-icons/fa';

/**
 * DS風のステータスバーコンポーネント
 * - 時刻表示（リアルタイム更新）
 * - ネットワーク名表示
 * - 音量とWi-Fiアイコン
 */
export default function StatusBar() {
    const [currentTime, setCurrentTime] = useState<string>('');

    useEffect(() => {
        const updateTime = () => {
            const now = new Date();
            const hours = now.getHours();
            const minutes = now.getMinutes().toString().padStart(2, '0');
            const ampm = hours >= 12 ? 'PM' : 'AM';
            const displayHours = hours % 12 || 12;
            setCurrentTime(`${displayHours}:${minutes} ${ampm}`);
        };

        updateTime();
        const interval = setInterval(updateTime, 1000);

        return () => clearInterval(interval);
    }, []);

    return (
        <div className="bg-gray-900 text-white px-4 py-2 flex items-center justify-between border-b-2 border-gray-700">
            {/* 左側: 音量アイコンと時刻 */}
            <div className="flex items-center gap-2">
                <FaVolumeUp className="text-xl" />
                <span className="font-medium text-xl">{currentTime}</span>
            </div>

            {/* 中央: ネットワーク名 */}
            <div className="font-bold text-xl ml-auto mr-4">
                PK - NETWORK
            </div>

            {/* 右側: Wi-Fiアイコン */}
            <div>
                <FaWifi className="text-2xl" style={{ color: '#4a9db8' }} />
            </div>
        </div>
    );
}
