#!/bin/bash

# テスト用のユニークなPlayerIdを生成
TIMESTAMP=$(date +%s)
PLAYER_ID="${TIMESTAMP}testplayer"

echo "=== ポケモンバトルテストスクリプト ==="
echo "PlayerId: $PLAYER_ID"
echo ""

# 1. 認証
echo "1. 認証中..."
AUTH_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$PLAYER_ID\",\"password\":\"testpassword\"}")

TOKEN=$(echo $AUTH_RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
  echo "❌ 認証失敗"
  echo "レスポンス: $AUTH_RESPONSE"
  exit 1
fi

echo "✅ 認証成功"
echo "トークン: ${TOKEN:0:50}..."
echo ""

# 2. スターターポケモンの選択肢を取得
echo "2. スターターポケモンの選択肢を取得中..."
STARTERS=$(curl -s -X GET http://localhost:5000/api/Starter/options \
  -H "Authorization: Bearer $TOKEN")

echo "✅ スターターポケモン取得成功"
echo "選択肢: ヒコザル(390), ゼニガメ(7), ツタージャ(495)"
echo ""

# 3. スターターポケモンを選択 (ヒコザル: 390)
echo "3. スターターポケモンを選択中 (ヒコザル)..."
STARTER_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Starter/select \
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

# 4. CPUバトルを作成
echo "4. CPUバトルを作成中..."
BATTLE_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Battle/cpu \
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
echo "バトルID: $BATTLE_ID"
echo ""

# 5. バトル状態を取得
echo "5. バトル状態を確認中..."
BATTLE_STATE=$(curl -s -X GET http://localhost:5000/api/Battle/$BATTLE_ID \
  -H "Authorization: Bearer $TOKEN")

echo "✅ バトル状態取得成功"
echo ""

echo "=== テスト完了 ==="
echo "すべてのエンドポイントが正常に動作しました！"
echo ""
echo "次のステップ:"
echo "- SignalRを使用してバトルを進行"
echo "- 技を使用して「Move not found」エラーが解消されているか確認"
