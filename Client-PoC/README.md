# PoC Battle Client

シンプルなポケモンバトル用クライアント（Proof of Concept）

## セットアップ

### 前提条件

- Docker & Docker Compose
- .NET 8.0 SDK (ローカル実行の場合)
- Python 3.x (PoCクライアント起動用)

### 1. JWT設定

#### appsettings.jsonの設定

`Server/src/Server.WebAPI/appsettings.json`に以下を追加:

```json
{
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long!",
    "Issuer": "PokeCloneAPI",
    "Audience": "PokeCloneClient"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=PokeCloneDb;User Id=sa;Password=Your_Password123!;TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  }
}
```

**重要**: `Jwt:Key`は最低32文字以上の安全なランダム文字列を使用してください。

### 2. 環境変数の設定

`.env`ファイル（プロジェクトルート）:

```bash
MSSQL_SA_PASSWORD=Your_Password123!
DATABASE_NAME=PokeCloneDb
REDIS_PASSWORD=
```

### 3. Docker Composeでの起動

```bash
cd /mnt/c/Users/cs23017/Shizuoka\ University/ドキュメント/dev/01_poke_clone-v3

# コンテナをビルド・起動
docker-compose up -d

# ログを確認
docker-compose logs -f app

# DBマイグレーションを適用
docker-compose exec app dotnet ef database update

# (オプション) 初期データがない場合、SeedDataを確認
docker-compose exec app dotnet ef database update
```

### 4. ローカル実行の場合

Docker Composeを使わずにローカルで実行する場合:

```bash
# SQL ServerとRedisをDockerで起動
docker-compose up -d db redis

# APIサーバーを起動
cd Server/src/Server.WebAPI
dotnet run
```

### 5. PoCクライアントの起動

```bash
cd Client-PoC
python -m http.server 8080
```

ブラウザで `http://localhost:8080` にアクセス

## バトルの作成とテスト

### APIでバトルを作成

#### 1. モック認証（開発用）

```bash
curl -X POST http://localhost:5000/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d '{"username":"player1","password":"test"}'
```

レスポンスからJWTトークンを取得。

#### 2. CPUバトルを作成

```bash
curl -X POST http://localhost:5000/api/Battle/cpu \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{"playerId":"player1"}'
```

レスポンスから`battleId`を取得。

#### 3. PoCクライアントで接続

1. **API URL**: `http://localhost:5000`
2. **Battle ID**: 上記で取得した`battleId`
3. **Player ID**: `player1`

「バトルに接続」をクリック

## 使用方法

### バトルフロー

1. 接続が成功すると、ポケモン情報とHPが表示されます
2. **技を選択**: 技1～4ボタンをクリック
3. **捕獲**: 捕獲ボタン（CPUバトルのみ）
4. **逃走**: 逃走ボタン（CPUバトルから逃げる）
5. バトルログで結果を確認

### SignalRイベント

- `BattleStarted`: 初期状態受信
- `ReceiveTurnResult`: ターン結果（ダメージ、効果など）
- `BattleEnded`: バトル終了（勝敗、捕獲、逃走）
- `BattleClosed`: 接続切断

## トラブルシューティング

### 接続できない

- サーバーが起動しているか確認: `docker-compose ps`
- ログを確認: `docker-compose logs app`
- Redisが起動しているか確認: `docker-compose ps redis`

### JWTエラー

- `appsettings.json`の`Jwt:Key`が32文字以上か確認
- JWTトークンが正しくリクエストヘッダーに含まれているか確認

### マイグレーションエラー

```bash
# マイグレーションを再適用
docker-compose exec app dotnet ef database update

# または、DBをリセット
docker-compose down -v
docker-compose up -d
docker-compose exec app dotnet ef database update
```

## 機能

- ✅ SignalRによるリアルタイム通信
- ✅ ポケモンのHP表示とアニメーション
- ✅ 技の使用
- ✅ 捕獲アクション
- ✅ 逃走アクション
- ✅ バトルログ表示
- ✅ 経験値獲得とレベルアップ（サーバー側）
- ✅ 進化処理（サーバー側）

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
- 認証はモック実装のみ

## 次のステップ

- ポケモン交代機能の追加
- ポケモン画像の表示
- 手持ちポケモン一覧の表示
- より詳細なバトルアニメーション
- 実際の認証システム統合

