# PoC Battle Client

シンプルなポケモンバトル用クライアント（Proof of Concept）

## 使用方法

### 1. サーバーの起動

```bash
cd /mnt/c/Users/cs23017/Shizuoka\ University/ドキュメント/dev/01_poke_clone-v3/Server/src/Server.WebAPI
dotnet run
```

### 2. クライアントの起動

簡易HTTPサーバーを使用:

```bash
cd /mnt/c/Users/cs23017/Shizuoka\ University/ドキュメント/dev/01_poke_clone-v3/Client-PoC
python -m http.server 8080
```

ブラウザで `http://localhost:8080` にアクセス

### 3. バトルへの接続

1. **API URL**: デフォルトで `http://localhost:5000`
2. **Battle ID**: バトルのID（例: `battle-uuid-1234`）※事前にAPIで作成が必要
3. **Player ID**: プレイヤーのID（例: `player1`）

「バトルに接続」ボタンをクリック

## 機能

- ✅ SignalRによるリアルタイム通信
- ✅ ポケモンのHP表示とアニメーション
- ✅ 技の使用
- ✅ 捕獲アクション
- ✅ 逃走アクション
- ✅ バトルログ表示

## 技術スタック

- HTML5
- CSS3 (グラデーション、アニメーション)
- jQuery 3.7.1
- SignalR Client 7.0.0

## 制限事項

- ポケモン交代機能は未実装
- 画像表示なし（簡易版）
- 手持ちポケモン情報の取得は未実装
- UIは最小限の機能のみ

## 次のステップ

- ポケモン交代機能の追加
- ポケモン画像の表示
- 手持ちポケモン一覧の表示
- より詳細なバトルアニメーション
