# Cluade

# 重要
- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## 作業内容

- seedsの位置に合わせて, seeds関連処理のパスの修正が必要.
- 最初のポケモンの選択肢を取得するエンドポイント
	- ヒコザル, ゼニガメ, ツタージャの情報を送信.
- 最初のポケモンを選択するエンドポイント
	- フロントエンドは選択したポケモンのSpeciesIdを送信.
	- プレイヤーがポケモンを所持していないか検証.
	- プレイヤーのパーティへポケモンを追加.
- パーティの一覧を取得するエンドポイントの実装.
	- 手持ち画面で使用.
- ポケモンを逃がすエンドポイント
	- プレイヤーの所持ポケモン > 1のとき, 任意のポケモンをパーティから逃がせる. 
- プレイヤーに所持金プロパティを追加.
	- クラス図を修正.
	- 戦闘終了後にお金を獲得する.
- ガチャエンドポイント
	- 所持金を消費してポケモンを取得可能.
- 捕獲処理の変更
	- プレイヤーの所持ポケモンが6体なら失敗するように修正.

### ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 