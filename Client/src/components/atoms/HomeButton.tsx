"use client";

import { useRouter } from "next/navigation";
import { FaHome } from "react-icons/fa";

import { useCommonTexts } from "@/hooks/usePageConfig";

/**
 * HOMEボタンコンポーネント
 * - ドット調（ピクセル風）のデザイン
 * - ルートページへ遷移
 */
export default function HomeButton() {
  const router = useRouter();
  const commonTexts = useCommonTexts();

  const handleClick = () => {
    router.push("/");
  };

  return (
    <button
      onClick={handleClick}
      className="flex items-center justify-center gap-2 border-2 border-gray-400 bg-gray-100 px-6 py-2 font-bold text-gray-800 transition-colors hover:bg-gray-200"
    >
      <FaHome className="text-lg" />
      <span>{commonTexts.home}</span>
    </button>
  );
}
