'use client';

import { useState } from 'react';

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
                <a href="https://pokeapi.co/" target="_blank" rel="noopener noreferrer" className="hover:underline">
                    PokeAPI
                </a>
                <a href="https://github.com/ayumu203/poke-clone-v3/" target="_blank" rel="noopener noreferrer" className="hover:underline">
                    Github
                </a>
                <a href="https://soundlibrary.pokemon.co.jp/" target="_blank" rel="noopener noreferrer" className="hover:underline">
                    BGM
                </a>
                <a href="https://booth.pm/ja/items/5677827" target="_blank" rel="noopener noreferrer" className="hover:underline">
                    背景画像
                </a>
            </nav>

            <div className="flex items-center gap-2">
                <span className="text-sm">BGM</span>
                <button
                    onClick={() => setIsBgmEnabled(!isBgmEnabled)}
                    className={`w-10 h-6 rounded-full transition-colors ${isBgmEnabled ? 'bg-green-400' : 'bg-gray-400'
                        } relative`}
                    aria-label="BGMトグル"
                >
                    <span
                        className={`absolute top-1 w-4 h-4 bg-white rounded-full transition-transform ${isBgmEnabled ? 'right-1' : 'left-1'
                            }`}
                    />
                </button>
            </div>
        </header>
    );
}
