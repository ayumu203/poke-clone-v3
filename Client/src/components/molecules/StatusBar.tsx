"use client";

import { useEffect, useRef, useState } from "react";
import { FaVolumeUp, FaWifi } from "react-icons/fa";

import VolumeSlider from "@/components/atoms/VolumeSlider";
import { useBGM } from "@/contexts/BGMContext";

/**
 * DS風のステータスバーコンポーネント
 * - 時刻表示（リアルタイム更新）
 * - ネットワーク名表示
 * - 音量とWi-Fiアイコン
 * - 音量調整スライダー
 */
export default function StatusBar() {
  const [currentTime, setCurrentTime] = useState<string>("");
  const [showVolumeSlider, setShowVolumeSlider] = useState(false);
  const { volume, setVolume } = useBGM();
  const sliderRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const updateTime = () => {
      const now = new Date();
      const hours = now.getHours();
      const minutes = now.getMinutes().toString().padStart(2, "0");
      const ampm = hours >= 12 ? "PM" : "AM";
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
      document.addEventListener("mousedown", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [showVolumeSlider]);

  return (
    <div className="flex items-center justify-between border-b-2 border-gray-700 bg-gray-900 px-4 py-2 text-white">
      {/* 左側: 音量アイコンと時刻 */}
      <div className="relative flex items-center gap-2">
        <button
          onClick={() => setShowVolumeSlider(!showVolumeSlider)}
          className="transition-colors hover:text-gray-300"
          aria-label="音量調整"
        >
          <FaVolumeUp className="text-xl" />
        </button>
        <span className="text-xl font-medium">{currentTime}</span>

        {/* 音量スライダー */}
        {showVolumeSlider && (
          <div
            ref={sliderRef}
            className="absolute top-full left-0 z-50 mt-2 rounded-lg border-2 border-gray-600 bg-gray-800 p-3 shadow-lg"
          >
            <VolumeSlider volume={volume} onVolumeChange={setVolume} />
          </div>
        )}
      </div>

      {/* 中央: ネットワーク名 */}
      <div className="mr-4 ml-auto text-xl font-bold">PK - NETWORK</div>

      {/* 右側: Wi-Fiアイコン */}
      <div>
        <FaWifi className="text-2xl" style={{ color: "#4a9db8" }} />
      </div>
    </div>
  );
}
