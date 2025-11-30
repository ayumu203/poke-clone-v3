# Cluade

# 重要
- 応答は日本語でお願いします.
- 不明点は毎度質問をお願いします.
- 作業内容は適宜Gitにて管理をお願いします(ブランチは現在のまま, コミットを行っていただければ大丈夫です).
- ドキュメントをよく確認してください.

## 作業内容

### 捕獲・逃走の実装

`/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Server/src/Server.WebAPI/Hubs/BattleHub.cs`のSubmitActionにて, 捕獲・逃走のロジックを実装する(他に良い実装先があれば要提案).
捕獲時のDBへの書き込みや捕獲・逃走成功時のSignalRの切断, Redisの削除などを実装する.

### 簡単な対戦用クライアントの作成(PoC)

- HTML, CSS, jQueryを使用して簡易な対戦用クライアントを作成する.
  - SignalRを利用した対戦
  - バトル状態の表示
  - プレイヤーの操作
  - 手持ちポケモン情報の取得
  - UIは簡素でいいです

### ドキュメント

- 特に以下のドキュメントをよく確認して作業をしてください.
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/Claude.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/シーケンス図.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/アーキテクチャ構成.drawio`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/フロントエンド実装手順.md`
  - `/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/要件定義.md` 