# PoC Battle Client

シンプルなポケモンバトル用クライアント（Proof of Concept）

## 前提条件

- Docker & Docker Compose
- Python 3.x (クライアント起動用)

## セットアップと起動

### 1. サーバーの起動（Docker Compose）

プロジェクトルートで以下を実行:

```bash
cd /mnt/c/Users/cs23017/Shizuoka\ University/ドキュメント/dev/01_poke_clone-v3

# コンテナをビルド・起動
docker compose up -d --build

# ログを確認
docker compose logs -f app
```

### 2. 初期データの注入

```bash
# 第1世代のポケモンと技をシード
docker compose run --rm app --seed --species --start 1 --end 151
docker compose run --rm app --seed --moves --start 1 --end 165
```

### 3. PoCクライアントの起動

別のターミナルで:

```bash
cd Client-PoC
python -m http.server 8080
```

ブラウザで `http://localhost:8080` にアクセス

## バトルの作成とテスト

### 1. 認証トークンの取得

```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d '{"username":"testplayer1","password":"testpassword"}' \
  | jq -r '.token')

# トークンの確認
echo "取得したトークン: $TOKEN"
```

### 2. プレイヤー情報の登録

```bash
curl -X POST http://localhost:5000/api/player/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "プレイヤー名",
    "iconUrl": "https://example.com/icon.png"
  }' | jq .
```

### 3. CPUバトルを作成

```bash
curl -X POST http://localhost:5000/api/Battle/cpu \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{"playerId":"testplayer1"}' | jq .
```

レスポンスから`battleId`を取得。

### 4. PoCクライアントで接続

1. **API URL**: `http://localhost:5000`
2. **Battle ID**: 上記で取得した`battleId`（例: `battle-xxx-xxx`）
3. **Player ID**: `testplayer1`

「バトルに接続」をクリック

## バトルの操作

1. 接続が成功すると、自分と相手のポケモン情報とHPが表示されます
2. **技を選択**: 技1～4ボタンをクリックして攻撃
3. **捕獲**: 捕獲ボタンをクリック（CPUバトルのみ、成功時はバトル終了）
4. **逃走**: 逃走ボタンをクリック（CPUバトルのみ、成功時はバトル終了）
5. バトルログで結果を確認

## トラブルシューティング

### 接続できない

```bash
# サーバーが起動しているか確認
docker compose ps

# アプリのログを確認
docker compose logs app

# Redisが起動しているか確認
docker compose ps redis
```

### データベースの確認

```bash
# ポケモンのデータ件数を確認
docker exec -it pokeclone_db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Your_Password123!' -d PokeCloneDb -C \
  -Q "SELECT COUNT(*) AS TotalCount FROM PokemonSpecies"

# 技のデータ件数を確認
docker exec -it pokeclone_db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Your_Password123!' -d PokeCloneDb -C \
  -Q "SELECT COUNT(*) AS TotalCount FROM Move"
```

### コンテナのリセット

```bash
# コンテナ、ボリューム、ネットワークをすべて削除
docker compose down -v

# 再起動
docker compose up -d --build
```

## Swagger UI

APIの詳細は以下で確認できます:
- <http://localhost:5000/swagger/index.html>

## 実装済み機能

- ✅ SignalRによるリアルタイム通信
- ✅ ポケモンのHP表示とアニメーション
- ✅ 技の使用（ダメージ、タイプ相性、急所判定）
- ✅ 捕獲アクション（成功時にDB保存、パーティ追加）
- ✅ 逃走アクション（CPUバトルのみ成功）
- ✅ バトルログ表示
- ✅ 経験値獲得とレベルアップ（サーバー側）
- ✅ 進化処理（サーバー側）

## 技術スタック

- **Client**: HTML5, CSS3, jQuery 3.7.1, SignalR Client 7.0.0
- **Server**: .NET 8, ASP.NET Core, SignalR, Entity Framework Core
- **Database**: SQL Server 2022, Redis 7

## 制約事項

- ポケモン交代機能は未実装
- ポケモン画像表示なし（簡易版）
- 手持ちポケモン一覧表示なし
- 認証はモック実装のみ

## 参考資料

詳細な起動方法は [`/Docs/起動方法等.md`](../Docs/起動方法等.md) を参照してください。
