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

# 4. スターターポケモンを選択
echo "4. スターターポケモンを選択中..."
# ヒコザル(390)を選択
STARTER_RESPONSE=$(curl -s -X POST $API_URL/api/Starter/select \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"pokemonSpeciesId":390}')

if echo "$STARTER_RESPONSE" | grep -q "error\|Error\|Exception"; then
    echo "エラー: スターターポケモンの選択に失敗しました"
    echo "レスポンス: $STARTER_RESPONSE"
    exit 1
fi

echo "✓ スターターポケモン選択成功"

# 5. CPUバトル作成
echo "5. CPUバトルを作成中..."
BATTLE_RESPONSE=$(curl -s -X POST $API_URL/api/Battle/cpu \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{}")

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
echo "Player ID: $PLAYER_ID"
echo ""
echo "PoCクライアントで上記の情報を使用してバトルに接続してください。"
