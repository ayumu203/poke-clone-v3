"use client";

import Image from "next/image";
import { useRouter } from "next/navigation";
import { type ReactNode } from "react";
import { type IconType } from "react-icons";

interface MenuButtonProps {
  icon?: IconType;
  imageSrc?: string;
  href: string;
  children: ReactNode;
}

/**
 * メニュー用ボタンコンポーネント
 * - 背景色: #333333
 * - 横幅小さめ、縦広め
 * - アイコンまたは画像 + テキスト（横並び）
 */
export default function MenuButton({ icon: Icon, imageSrc, href, children }: MenuButtonProps) {
  const router = useRouter();

  const handleClick = () => {
    router.push(href);
  };

  return (
    <button
      onClick={handleClick}
      className="relative flex w-full max-w-xs items-center justify-center border-2 border-gray-600 px-4 py-3 font-bold text-white transition-colors hover:bg-gray-700 sm:px-6 sm:py-5"
      style={{
        backgroundColor: "#333333",
        boxShadow: "inset 2px 2px 0 1px white, inset -2px -2px 0 1px white",
      }}
    >
      {Icon && (
        <div className="absolute left-[2px] flex items-center sm:left-4">
          <Icon className="text-2xl sm:text-4xl" />
        </div>
      )}
      {imageSrc && (
        <div className="absolute left-[2px] h-8 w-8 bg-transparent sm:left-4 sm:h-12 sm:w-12">
          <Image src={imageSrc} alt="" fill className="object-contain" />
        </div>
      )}
      <span className="text-[16px] leading-tight sm:text-[25px]">{children}</span>
    </button>
  );
}
