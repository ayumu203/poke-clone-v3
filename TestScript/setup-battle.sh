#!/bin/bash
# Unified Battle Setup and Test Script

set -e

API_URL="http://localhost:5000"

# テスト用のユニークなPlayerIdを生成
TIMESTAMP=$(date +%s)
PLAYER_ID="${TIMESTAMP}testplayer"

echo "=== ポケモンバトルセットアップ ===" 
echo "PlayerId: $PLAYER_ID"
echo ""

# 1. 認証
echo "1. 認証中..."
AUTH_RESPONSE=$(curl -s -X POST $API_URL/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$PLAYER_ID\",\"password\":\"testpassword\"}")

TOKEN=$(echo $AUTH_RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "❌ 認証失敗"
  echo "レスポンス: $AUTH_RESPONSE"
  exit 1
fi

echo "✅ 認証成功"
echo ""

# 2. スターターポケモンの選択肢を取得
echo "2. スターターポケモンの選択肢を取得中..."
STARTERS=$(curl -s -X GET $API_URL/api/Starter/options \
  -H "Authorization: Bearer $TOKEN")

echo "✅ スターターポケモン取得成功"
echo "選択肢: ヒコザル(390), ゼニガメ(7), ツタージャ(495)"
echo ""

# 3. プレイヤープロフィールを作成
echo "3. プレイヤープロフィールを作成中..."
PLAYER_RESPONSE=$(curl -s -X POST $API_URL/api/player/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"TestPlayer_${TIMESTAMP}\",\"iconUrl\":\"https://example.com/icon.png\"}")

if echo "$PLAYER_RESPONSE" | grep -q "error\|Error\|Exception"; then
  echo "❌ プレイヤープロフィール作成失敗"
  echo "レスポンス: ${PLAYER_RESPONSE:0:500}"
  exit 1
fi

echo "✅ プレイヤープロフィール作成成功"
echo ""

# 4. スターターポケモンを選択 (ヒコザル: 390)
echo "4. スターターポケモンを選択中 (ヒコザル)..."
STARTER_RESPONSE=$(curl -s -X POST $API_URL/api/Starter/select \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"pokemonSpeciesId":390}')

if echo "$STARTER_RESPONSE" | grep -q "error\|Error\|Exception"; then
  echo "❌ スターターポケモン選択失敗"
  echo "レスポンス: ${STARTER_RESPONSE:0:500}"
  exit 1
fi

echo "✅ スターターポケモン選択成功"
echo ""

# 5. CPUバトルを作成
echo "5. CPUバトルを作成中..."
BATTLE_RESPONSE=$(curl -s -X POST $API_URL/api/Battle/cpu \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json")

if echo "$BATTLE_RESPONSE" | grep -q "error\|Error\|Exception"; then
  echo "❌ バトル作成失敗"
  echo "レスポンス: ${BATTLE_RESPONSE:0:500}"
  exit 1
fi

BATTLE_ID=$(echo $BATTLE_RESPONSE | grep -o '"battleId":"[^"]*"' | cut -d'"' -f4)

if [ -z "$BATTLE_ID" ]; then
  echo "❌ バトルID取得失敗"
  echo "レスポンス: ${BATTLE_RESPONSE:0:500}"
  exit 1
fi

echo "✅ バトル作成成功"
echo ""

# 6. バトル状態を取得
echo "6. バトル状態を確認中..."
BATTLE_STATE=$(curl -s -X GET $API_URL/api/Battle/$BATTLE_ID \
  -H "Authorization: Bearer $TOKEN")

echo "✅ バトル状態取得成功"
echo ""

# 結果表示
echo "=== セットアップ完了 ==="
echo "API URL: $API_URL"
echo "Battle ID: $BATTLE_ID"
echo "Player ID: $PLAYER_ID"
echo ""
echo "PoCクライアントで上記の情報を使用してバトルに接続してください。"
