"use client";

import { useEffect } from "react";

import ExternalLink from "@/components/atoms/ExternalLink";
import ToggleSwitch from "@/components/atoms/ToggleSwitch";
import { useBGM } from "@/contexts/BGMContext";
import headerData from "@/data/common/header.json";
import { useAudio } from "@/hooks/useAudio";
import { useCommonTexts } from "@/hooks/usePageConfig";

interface HeaderProps {
  bgmSrc?: string;
}

/**
 * ヘッダーコンポーネント
 * - 背景色: 青
 * - 外部リンク
 * - BGMトグルスイッチ（BGM再生制御）
 */
export default function Header({ bgmSrc = "/audio/01_opening.mp3" }: HeaderProps) {
  const { isBgmEnabled, setIsBgmEnabled, volume } = useBGM();
  const commonTexts = useCommonTexts();
  const { play, pause, setVolume } = useAudio({
    src: bgmSrc,
    loop: true,
    volume: volume / 100,
    autoPlay: false,
  });

  // BGMトグルの変更を監視
  useEffect(() => {
    if (isBgmEnabled) {
      play().catch((error) => {
        console.error("BGM再生エラー:", error);
        setIsBgmEnabled(false);
      });
    } else {
      pause();
    }
  }, [isBgmEnabled, play, pause, setIsBgmEnabled]);

  // 音量の変更を監視
  useEffect(() => {
    setVolume(volume / 100);
  }, [volume, setVolume]);

  return (
    <header className="flex w-full items-center justify-between bg-blue-600 px-4 py-2 text-white">
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
        label={commonTexts.bgm}
      />
    </header>
  );
}
