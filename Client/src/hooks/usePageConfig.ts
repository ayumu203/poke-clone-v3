import pageConfig from '@/data/pages/config.json';

interface MenuButton {
    text: string;
    icon: string;
    href: string;
}

interface PageConfig {
    image: string;
    bgm: string;
    menuButtons?: MenuButton[];
}

type PageName = 'login' | 'home' | 'cpu-battle' | 'online-battle' | 'party' | 'gacha' | 'starter-pokemon';

/**
 * ページ設定を取得するフック
 * @param pageName ページ名
 * @returns ページの画像とBGMの設定
 */
export function usePageConfig(pageName: PageName): PageConfig {
    const config = pageConfig as Record<PageName, PageConfig>;
    return config[pageName] || config.home;
}
