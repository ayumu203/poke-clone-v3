'use client';

import { useEffect } from 'react';

import ExternalLink from '@/components/atoms/ExternalLink';
import ToggleSwitch from '@/components/atoms/ToggleSwitch';
import { useBGM } from '@/contexts/BGMContext';
import headerData from '@/data/common/header.json';
import { useAudio } from '@/hooks/useAudio';

interface HeaderProps {
    bgmSrc?: string;
}

/**
 * ヘッダーコンポーネント
 * - 背景色: 青
 * - 外部リンク
 * - BGMトグルスイッチ（BGM再生制御）
 */
export default function Header({ bgmSrc = '/audio/01_opening.mp3' }: HeaderProps) {
    const { isBgmEnabled, setIsBgmEnabled } = useBGM();
    const { play, pause } = useAudio({
        src: bgmSrc,
        loop: true,
        volume: 0.1,
        autoPlay: false,
    });

    // BGMトグルの変更を監視
    useEffect(() => {
        if (isBgmEnabled) {
            play().catch((error) => {
                console.error('BGM再生エラー:', error);
                setIsBgmEnabled(false);
            });
        } else {
            pause();
        }
    }, [isBgmEnabled, play, pause, setIsBgmEnabled]);

    return (
        <header className="w-full bg-blue-600 text-white px-4 py-2 flex items-center justify-between">
            <nav className="flex gap-4 text-sm">
                {headerData.externalLinks.map((link) => (
                    <ExternalLink key={link.name} href={link.url}>
                        {link.name}
                    </ExternalLink>
                ))}
            </nav>

            <ToggleSwitch
                enabled={isBgmEnabled}
                onToggle={() => setIsBgmEnabled(!isBgmEnabled)}
                label="BGM"
            />
        </header>
    );
}
