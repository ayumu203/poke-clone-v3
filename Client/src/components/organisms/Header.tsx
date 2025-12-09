'use client';

import { useState } from 'react';
import ExternalLink from '@/components/atoms/ExternalLink';
import ToggleSwitch from '@/components/atoms/ToggleSwitch';
import headerData from '@/data/common/header.json';

/**
 * ヘッダーコンポーネント
 * - 背景色: 青
 * - 外部リンク
 * - BGMトグルスイッチ（ダミー）
 */
export default function Header() {
    const [isBgmEnabled, setIsBgmEnabled] = useState(false);

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
