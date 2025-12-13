'use client';

import { useEffect, useRef, useState } from 'react';
import { FaVolumeUp, FaWifi } from 'react-icons/fa';

import VolumeSlider from '@/components/atoms/VolumeSlider';
import { useBGM } from '@/contexts/BGMContext';

/**
 * DS風のステータスバーコンポーネント
 * - 時刻表示（リアルタイム更新）
 * - ネットワーク名表示
 * - 音量とWi-Fiアイコン
 * - 音量調整スライダー
 */
export default function StatusBar() {
    const [currentTime, setCurrentTime] = useState<string>('');
    const [showVolumeSlider, setShowVolumeSlider] = useState(false);
    const { volume, setVolume } = useBGM();
    const sliderRef = useRef<HTMLDivElement>(null);

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

    // 外側をクリックしたらスライダーを閉じる
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (sliderRef.current && !sliderRef.current.contains(event.target as Node)) {
                setShowVolumeSlider(false);
            }
        };

        if (showVolumeSlider) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [showVolumeSlider]);

    return (
        <div className="bg-gray-900 text-white px-4 py-2 flex items-center justify-between border-b-2 border-gray-700">
            {/* 左側: 音量アイコンと時刻 */}
            <div className="flex items-center gap-2 relative">
                <button
                    onClick={() => setShowVolumeSlider(!showVolumeSlider)}
                    className="hover:text-gray-300 transition-colors"
                    aria-label="音量調整"
                >
                    <FaVolumeUp className="text-xl" />
                </button>
                <span className="font-medium text-xl">{currentTime}</span>

                {/* 音量スライダー */}
                {showVolumeSlider && (
                    <div
                        ref={sliderRef}
                        className="absolute left-0 top-full mt-2 bg-gray-800 border-2 border-gray-600 rounded-lg p-3 shadow-lg z-50"
                    >
                        <VolumeSlider volume={volume} onVolumeChange={setVolume} />
                    </div>
                )}
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
