'use client';

import { createContext, useContext, useState, ReactNode, useEffect } from 'react';

interface BGMContextType {
    isBgmEnabled: boolean;
    setIsBgmEnabled: (enabled: boolean) => void;
}

const BGMContext = createContext<BGMContextType | undefined>(undefined);

export function BGMProvider({ children }: { children: ReactNode }) {
    const [isBgmEnabled, setIsBgmEnabled] = useState(false);

    // localStorageから初期値を読み込む
    useEffect(() => {
        const saved = localStorage.getItem('bgm-enabled');
        if (saved !== null) {
            setIsBgmEnabled(saved === 'true');
        }
    }, []);

    // 状態が変更されたらlocalStorageに保存
    useEffect(() => {
        localStorage.setItem('bgm-enabled', String(isBgmEnabled));
    }, [isBgmEnabled]);

    return (
        <BGMContext.Provider value={{ isBgmEnabled, setIsBgmEnabled }}>
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
