#!/bin/bash
# PoC Battle Setup Script

set -e

API_URL="http://localhost:5000"
DB_CONTAINER="pokeclone_db"
DB_PASSWORD="Your_Password123!"

echo "=== PoC Battle Setup ==="

# 1. トークン取得
echo "1. 認証トークンを取得中..."
TOKEN=$(curl -s -X POST $API_URL/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d '{"username":"testplayer1","password":"testpassword"}' \
  | jq -r '.token')

if [ -z "$TOKEN" ] || [ "$TOKEN" = "null" ]; then
    echo "エラー: トークンの取得に失敗しました"
    exit 1
fi

echo "✓ トークン取得成功"

# 2. プレイヤーを作成
echo "2. プレイヤーを作成中..."
curl -s -X POST $API_URL/api/player/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"TestPlayer","iconUrl":"https://example.com/icon.png"}' > /dev/null

echo "✓ プレイヤー作成成功"

# 3. プレイヤーID取得
echo "3. プレイヤーIDを取得中..."
PLAYER_ID=$(curl -s -X GET $API_URL/api/player/me \
  -H "Authorization: Bearer $TOKEN" | jq -r '.playerId')

if [ -z "$PLAYER_ID" ] || [ "$PLAYER_ID" = "null" ]; then
    echo "エラー: プレイヤーIDの取得に失敗しました"
    exit 1
fi

echo "✓ プレイヤーID: $PLAYER_ID"

# 4. ポケモンをパーティに追加
echo "4. ポケモンをパーティに追加中..."
POKEMON_ID=$(docker exec -i $DB_CONTAINER /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "$DB_PASSWORD" -d PokeCloneDb -C -h -1 -Q "
  SET NOCOUNT ON;
  DECLARE @PokemonId NVARCHAR(255) = NEWID();
  DECLARE @PlayerId NVARCHAR(255) = '$PLAYER_ID';
  
  INSERT INTO Pokemon (pokemonId, pokemonSpeciesId, level, exp)
  VALUES (@PokemonId, 1, 5, 0);

  INSERT INTO PokemonMoveInstance (pokemonId, moveId)
  SELECT TOP 4 @PokemonId, moveId FROM PokemonMove WHERE pokemonSpeciesId = 1;

  DECLARE @PartyId INT;
  SELECT @PartyId = playerPartyId FROM PlayerParty WHERE playerId = @PlayerId;

  IF @PartyId IS NULL
  BEGIN
    INSERT INTO PlayerParty (playerId) VALUES (@PlayerId);
    SET @PartyId = SCOPE_IDENTITY();
  END

  INSERT INTO PlayerPartyPokemon (playerPartyId, pokemonId)
  VALUES (@PartyId, @PokemonId);

  SELECT @PokemonId;
" | tr -d '[:space:]')

echo "✓ ポケモンID: $POKEMON_ID"

# 5. CPUバトル作成
echo "5. CPUバトルを作成中..."
BATTLE_RESPONSE=$(curl -s -X POST $API_URL/api/Battle/cpu \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"playerId\":\"$PLAYER_ID\"}")

BATTLE_ID=$(echo $BATTLE_RESPONSE | jq -r '.battleId')

if [ -z "$BATTLE_ID" ] || [ "$BATTLE_ID" = "null" ]; then
    echo "エラー: バトルの作成に失敗しました"
    echo "レスポンス: $BATTLE_RESPONSE"
    exit 1
fi

echo "✓ バトルID: $BATTLE_ID"

# 6. 結果表示
echo ""
echo "=== セットアップ完了 ==="
echo "API URL: $API_URL"
echo "Battle ID: $BATTLE_ID"
echo "Player ID: testplayer1"
echo ""
echo "PoCクライアントで上記の情報を使用してバトルに接続してください。"
