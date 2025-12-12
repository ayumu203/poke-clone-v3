'use client';

import { useState } from 'react';

interface VolumeSliderProps {
    volume: number;
    onVolumeChange: (volume: number) => void;
    className?: string;
}

/**
 * 音量調整用スライダーコンポーネント
 * - 0-100%の範囲で音量を調整
 * - リアルタイムで音量を反映
 */
export default function VolumeSlider({ volume, onVolumeChange, className = '' }: VolumeSliderProps) {
    const [_isDragging, setIsDragging] = useState(false);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const newVolume = parseInt(e.target.value, 10);
        onVolumeChange(newVolume);
    };

    return (
        <div className={`flex items-center gap-2 ${className}`}>
            <span className="text-sm font-medium min-w-[2rem]">{volume}%</span>
            <input
                type="range"
                min="0"
                max="100"
                value={volume}
                onChange={handleChange}
                onMouseDown={() => setIsDragging(true)}
                onMouseUp={() => setIsDragging(false)}
                onTouchStart={() => setIsDragging(true)}
                onTouchEnd={() => setIsDragging(false)}
                className="w-32 h-2 bg-gray-700 rounded-lg appearance-none cursor-pointer slider"
                style={{
                    background: `linear-gradient(to right, #4a9db8 0%, #4a9db8 ${volume}%, #374151 ${volume}%, #374151 100%)`,
                }}
            />
            <style>{`
                .slider::-webkit-slider-thumb {
                    appearance: none;
                    width: 16px;
                    height: 16px;
                    background: #4a9db8;
                    border-radius: 50%;
                    cursor: pointer;
                }
                .slider::-moz-range-thumb {
                    width: 16px;
                    height: 16px;
                    background: #4a9db8;
                    border-radius: 50%;
                    cursor: pointer;
                    border: none;
                }
            `}</style>
        </div>
    );
}
