interface MessageScreenProps {
  message: string;
  messageClassName?: string;
}

/**
 * メッセージを中央表示する汎用コンポーネント
 * - ローディング画面、完了画面、エラー画面などに使用
 */
export default function MessageScreen({
  message,
  messageClassName = "text-2xl font-bold text-center px-8",
}: MessageScreenProps) {
  return (
    <div className="flex h-full items-center justify-center">
      <p className={messageClassName}>{message}</p>
    </div>
  );
}
