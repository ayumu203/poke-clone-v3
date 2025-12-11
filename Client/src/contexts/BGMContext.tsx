'use client';

import { createContext, useContext, useState, ReactNode, useEffect } from 'react';

interface BGMContextType {
    isBgmEnabled: boolean;
    setIsBgmEnabled: (enabled: boolean) => void;
    volume: number;
    setVolume: (volume: number) => void;
}

const BGMContext = createContext<BGMContextType | undefined>(undefined);

export function BGMProvider({ children }: { children: ReactNode }) {
    const [isBgmEnabled, setIsBgmEnabled] = useState(false);
    const [volume, setVolumeState] = useState(50); // デフォルト50%

    // localStorageから初期値を読み込む
    useEffect(() => {
        const saved = localStorage.getItem('bgm-enabled');
        if (saved !== null) {
            setIsBgmEnabled(saved === 'true');
        }

        const savedVolume = localStorage.getItem('bgm-volume');
        if (savedVolume !== null) {
            setVolumeState(parseInt(savedVolume, 10));
        }
    }, []);

    // 状態が変更されたらlocalStorageに保存
    useEffect(() => {
        localStorage.setItem('bgm-enabled', String(isBgmEnabled));
    }, [isBgmEnabled]);

    useEffect(() => {
        localStorage.setItem('bgm-volume', String(volume));
    }, [volume]);

    const setVolume = (newVolume: number) => {
        setVolumeState(Math.max(0, Math.min(100, newVolume)));
    };

    return (
        <BGMContext.Provider value={{ isBgmEnabled, setIsBgmEnabled, volume, setVolume }}>
            {children}
        </BGMContext.Provider>
    );
}

export function useBGM() {
    const context = useContext(BGMContext);
    if (context === undefined) {
        throw new Error('useBGM must be used within a BGMProvider');
    }
    return context;
}
