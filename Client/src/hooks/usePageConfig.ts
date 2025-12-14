import pageConfig from "@/data/pages/config.json";

interface MenuButton {
  text: string;
  icon: string;
  href: string;
}

interface LoginTexts {
  accountRequired: string;
  googleLogin: string;
  googleLoading: string;
  emailLogin: string;
  alreadyLoggedIn: string;
  loginTab: string;
  signupTab: string;
  loginTitle: string;
  signupTitle: string;
  emailPlaceholder: string;
  passwordPlaceholder: string;
  passwordHint: string;
  loginButton: string;
  signupButton: string;
  loginLoading: string;
  signupLoading: string;
  cancel: string;
  errors: {
    invalidCredential: string;
    emailInUse: string;
    weakPassword: string;
    invalidEmail: string;
    unexpected: string;
  };
}

interface StarterTexts {
  title: string;
  alreadyHasParty: string;
  selectionComplete: string;
  selecting: string;
}

interface PartyTexts {
  loading: string;
  error: string;
  release: string;
  releaseConfirm: string;
  releaseCancel: string;
  back: string;
  stats: {
    attack: string;
    defense: string;
    spAttack: string;
    spDefense: string;
    speed: string;
  };
}

interface CommonTexts {
  loading: string;
  logout: string;
  loggingOut: string;
  home: string;
  bgm: string;
}

interface PageConfigBase {
  image: string;
  bgm: string;
  menuButtons?: MenuButton[];
}

interface LoginPageConfig extends PageConfigBase {
  t: LoginTexts;
}

interface StarterPageConfig extends PageConfigBase {
  t: StarterTexts;
}

interface PartyPageConfig extends PageConfigBase {
  t: PartyTexts;
}

type PageName =
  | "login"
  | "home"
  | "cpu-battle"
  | "online-battle"
  | "party"
  | "gacha"
  | "starter-pokemon";

type ConfigData = {
  common: CommonTexts;
  login: LoginPageConfig;
  home: PageConfigBase & { menuButtons: MenuButton[] };
  "cpu-battle": PageConfigBase;
  "online-battle": PageConfigBase;
  party: PartyPageConfig;
  gacha: PageConfigBase;
  "starter-pokemon": StarterPageConfig;
};

const config = pageConfig as unknown as ConfigData;

/**
 * ページ設定を取得するフック
 * @param pageName ページ名
 * @returns ページの画像とBGMの設定
 */
export function usePageConfig<T extends PageName>(pageName: T): ConfigData[T] {
  return config[pageName];
}

/**
 * 共通テキストを取得するフック
 * @returns 共通テキスト設定
 */
export function useCommonTexts(): CommonTexts {
  return config.common;
}
