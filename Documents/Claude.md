# Claude

## 重要

- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- コミットメッセージの形式: `prefix: コミットメッセージ(日本語)`
- ドキュメントをよく確認してください.
- 不要になったコメント・スクリプトは適宜削除してください.

## 内容

- / でユーザーusePartyを利用し, ユーザのパーティ情報を取得してください.
- ユーザのパーティ情報が存在しない場合は/starter-pokemonにリダイレクトしてください.
- ユーザのパーティ情報が存在する場合は通常のホーム画面を表示してください.
- /starter-pokemonでは, 以下のように簡易体な実装をしてください.
  - useStarterにてスターターポケモンの選択肢を取得.
  - 下画面に各選択肢に対応するボタンを表示.
  - ボタンを押すとuseStarterにて選択肢を送信し, 登録完了後に「選択完了と表示」.
  - コンソールにusePartyにてパーティ情報を取得して表示してください.

## ドキュメント

- 特に以下の資料をよく確認して作業をしてください.
  - `/home/cs23017/poke-clone-v3/Documents/Claude.md`
  - `/home/cs23017/poke-clone-v3/Documents/UML/クラス図.drawio`
  - `/home/cs23017/poke-clone-v3/Documents/UML/シーケンス図.drawio`
  - `/home/cs23017/poke-clone-v3/Documents/UML/アーキテクチャ構成.drawio`
  - `/home/cs23017/poke-clone-v3/Documents/フロントエンド実装手順.md`
  - `/home/cs23017/poke-clone-v3/Documents/要件定義.md` 