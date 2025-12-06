# Claude

## 重要

- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## 内容

- 実装予定.mdの以下に関して回答・修正をお願いします.
  - `appsettings.json`, `appsettings.Development.json`に含まれている接続情報は実装予定.mdを実施することで隠匿できますか？
  - フロントエンドの認証(Google, Microsoft, メール)のAzure External Entra IDに関する記述が見当たりません.
  - フロントエンドのレンダリング方法を検討したいです.
    - フロントエンドのページ構成としては`/home/cs23017/poke-clone-v3/Design/ポケモン詳細画面.svg`のように, 上画面に`/home/cs23017/poke-clone-v3/Documents/Images`の画像, 下画面にボタンや情報を表示, 画面外のヘッダーに各リンク, BGMのON/OFFのトグルスイッチを配置.

(1) フロントエンドディレクトリ構造

```bash
/poke-clone-v3/Client$ tree -I "node_modules"
.
├── README.md
├── debug-storybook.log
├── eslint.config.mjs
├── next-env.d.ts
├── next.config.ts
├── package.json
├── pnpm-lock.yaml
├── postcss.config.mjs
├── prettier.config.mjs
├── public
├── src
│   ├── app
│   │   ├── favicon.ico
│   │   ├── globals.css
│   │   ├── layout.tsx
│   │   └── page.tsx
│   ├── components
│   │   ├── atoms
│   │   ├── molecules
│   │   ├── organisms
│   │   └── templates
│   ├── hooks
│   ├── lib
│   │   └── api
│   │       └── schemas.ts
│   ├── stories
│   │   └── assets
│   ├── types
│   └── utils
├── tsconfig.json
├── vitest.config.ts
└── vitest.shims.d.ts
```

## ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 