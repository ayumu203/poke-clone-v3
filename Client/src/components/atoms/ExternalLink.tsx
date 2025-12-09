interface ExternalLinkProps {
    href: string;
    children: React.ReactNode;
}

/**
 * 外部リンクコンポーネント
 * - 新しいタブで開く
 * - セキュアな設定（noopener noreferrer）
 * - ホバー時にアンダーライン
 */
export default function ExternalLink({ href, children }: ExternalLinkProps) {
    return (
        <a
            href={href}
            target="_blank"
            rel="noopener noreferrer"
            className="hover:underline"
        >
            {children}
        </a>
    );
}
