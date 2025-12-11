'use client';

import { createContext, useContext, ReactNode } from 'react';

import { useLocalStorage } from '@/hooks/useLocalStorage';

interface BGMContextType {
    isBgmEnabled: boolean;
    setIsBgmEnabled: (enabled: boolean) => void;
    volume: number;
    setVolume: (volume: number) => void;
}

const BGMContext = createContext<BGMContextType | undefined>(undefined);

export function BGMProvider({ children }: { children: ReactNode }) {
    const [isBgmEnabled, setIsBgmEnabled] = useLocalStorage('bgm-enabled', false);
    const [volume, setVolumeState] = useLocalStorage('bgm-volume', 50);

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
