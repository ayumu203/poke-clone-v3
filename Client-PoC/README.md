# PoC Battle Client

## 環境

- Docker & Docker Compose
- Python 3.x

## セットアップと起動

### 1. サーバーの起動

プロジェクトルートで以下を実行:

```bash
# プロジェクトのルートに移動

# コンテナをビルド・起動
docker compose up -d --build

# ログを確認
docker compose logs -f app
```

### 2. PoCクライアントの起動

```bash
cd Client-PoC
python3 -m http.server 8080
```

ブラウザで `http://localhost:8080` にアクセス

## バトルの作成とテスト

### 簡単な方法（推奨）

スクリプトを使用して自動的にセットアップできます:

```bash
# プロジェクトルートから実行
cd scripts
chmod +x setup-battle.sh
./setup-battle.sh
```

スクリプトが以下の情報を出力します:
- **API URL**: `http://localhost:5000`
- **Battle ID**: バトルID（例: `battle-xxx-xxx`）
- **Player ID**: `testplayer1`

これらの情報をPoCクライアントで使用してバトルに接続してください。

---

### 詳細な手順（手動セットアップ）

手動でセットアップする場合は、以下の手順に従ってください。

#### 1. 認証トークンの取得

```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d '{"username":"testplayer1","password":"testpassword"}' \
  | jq -r '.token')

# トークンの確認
echo "取得したトークン: $TOKEN"
```

#### 2. プレイヤー情報の登録

```bash
curl -X POST http://localhost:5000/api/player/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "プレイヤー名",
    "iconUrl": "https://example.com/icon.png"
  }' | jq .
```

レスポンスから`playerId`を取得してください（例: `b0764a84-bed5-4ef6-83c3-09b707e5ae40`）

#### 3. ポケモンをパーティに追加

**重要**: JWTトークンに含まれる実際のplayerIdを使用してください。

```bash
# JWTトークンからplayerIdを抽出
PLAYER_ID=$(curl -s -X GET http://localhost:5000/api/player/me \
  -H "Authorization: Bearer $TOKEN" | jq -r '.playerId')

echo "Player ID: $PLAYER_ID"

# ポケモンを追加（実際のplayerIdを使用）
docker exec -it pokeclone_db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P 'Your_Password123!' -d PokeCloneDb -C -Q "
  -- Pokemonを作成
  DECLARE @PokemonId NVARCHAR(255) = NEWID();
  DECLARE @PlayerId NVARCHAR(255) = '$PLAYER_ID';
  
  INSERT INTO Pokemon (pokemonId, pokemonSpeciesId, level, exp)
  VALUES (@PokemonId, 1, 5, 0);

  -- Pokemon用の技を追加
  INSERT INTO PokemonMoveInstance (pokemonId, moveId)
  SELECT TOP 4 @PokemonId, moveId FROM PokemonMove WHERE pokemonSpeciesId = 1;

  -- PlayerPartyを作成または取得
  DECLARE @PartyId INT;
  SELECT @PartyId = playerPartyId FROM PlayerParty WHERE playerId = @PlayerId;

  IF @PartyId IS NULL
  BEGIN
    INSERT INTO PlayerParty (playerId) VALUES (@PlayerId);
    SET @PartyId = SCOPE_IDENTITY();
  END

  -- Pokemonをパーティに追加
  INSERT INTO PlayerPartyPokemon (playerPartyId, pokemonId)
  VALUES (@PartyId, @PokemonId);

  SELECT 'Pokemon added successfully. Pokemon ID: ' + @PokemonId AS Result;
  "
```

#### 4. CPUバトルを作成

```bash
# PLAYER_IDを使用してバトルを作成
curl -X POST http://localhost:5000/api/Battle/cpu \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"playerId\":\"$PLAYER_ID\"}" | jq .
```

レスポンスから`battleId`を取得。

#### 5. PoCクライアントで接続

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
