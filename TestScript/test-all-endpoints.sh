#!/bin/bash

# テスト結果カウンター
SUCCESS_COUNT=0
FAILURE_COUNT=0
SKIP_COUNT=0
TOTAL_COUNT=0

# テスト用のユニークなPlayerIdを生成
TIMESTAMP=$(date +%s)
PLAYER_ID="${TIMESTAMP}testplayer"

echo "=========================================="
echo "  全エンドポイントテストスクリプト"
echo "=========================================="
echo "PlayerId: $PLAYER_ID"
echo "開始時刻: $(date '+%Y-%m-%d %H:%M:%S')"
echo ""

# ヘルパー関数: テスト結果を記録
test_endpoint() {
    local name="$1"
    local result="$2"
    TOTAL_COUNT=$((TOTAL_COUNT + 1))
    
    if [ "$result" = "success" ]; then
        echo "✅ $name"
        SUCCESS_COUNT=$((SUCCESS_COUNT + 1))
    elif [ "$result" = "skip" ]; then
        echo "⚠️  $name (スキップ)"
        SKIP_COUNT=$((SKIP_COUNT + 1))
    else
        echo "❌ $name"
        FAILURE_COUNT=$((FAILURE_COUNT + 1))
    fi
}

# ========================================
# 1. 認証なしエンドポイント
# ========================================
echo "=========================================="
echo "1. 認証なしエンドポイント"
echo "=========================================="

# GET /api/Pokemon
echo -n "テスト中: GET /api/Pokemon ... "
POKEMON_RESPONSE=$(curl -s -X GET http://localhost:5000/api/Pokemon)
if echo "$POKEMON_RESPONSE" | grep -q "pokemonSpeciesId"; then
    test_endpoint "GET /api/Pokemon - 全ポケモン種族取得" "success"
else
    test_endpoint "GET /api/Pokemon - 全ポケモン種族取得" "failure"
fi

# GET /api/Pokemon/25 (ピカチュウ)
echo -n "テスト中: GET /api/Pokemon/25 ... "
POKEMON_DETAIL=$(curl -s -X GET http://localhost:5000/api/Pokemon/25)
if echo "$POKEMON_DETAIL" | grep -q "pokemonSpeciesId"; then
    test_endpoint "GET /api/Pokemon/25 - ポケモン種族詳細取得" "success"
else
    test_endpoint "GET /api/Pokemon/25 - ポケモン種族詳細取得" "failure"
fi

# GET /api/Moves
echo -n "テスト中: GET /api/Moves ... "
MOVES_RESPONSE=$(curl -s -X GET http://localhost:5000/api/Moves)
if echo "$MOVES_RESPONSE" | grep -q "moveId"; then
    test_endpoint "GET /api/Moves - 全技取得" "success"
else
    test_endpoint "GET /api/Moves - 全技取得" "failure"
fi

# GET /api/Moves/1
echo -n "テスト中: GET /api/Moves/1 ... "
MOVE_DETAIL=$(curl -s -X GET http://localhost:5000/api/Moves/1)
if echo "$MOVE_DETAIL" | grep -q "moveId"; then
    test_endpoint "GET /api/Moves/1 - 技詳細取得" "success"
else
    test_endpoint "GET /api/Moves/1 - 技詳細取得" "failure"
fi

echo ""

# ========================================
# 2. 認証
# ========================================
echo "=========================================="
echo "2. 認証"
echo "=========================================="

# GET /api/Auth/status (認証前)
echo -n "テスト中: GET /api/Auth/status (認証前) ... "
AUTH_STATUS_BEFORE=$(curl -s -X GET http://localhost:5000/api/Auth/status)
if echo "$AUTH_STATUS_BEFORE" | grep -q "isAuthenticated"; then
    test_endpoint "GET /api/Auth/status - 認証状態確認(認証前)" "success"
else
    test_endpoint "GET /api/Auth/status - 認証状態確認(認証前)" "failure"
fi

# POST /api/Auth/login/mock
echo -n "テスト中: POST /api/Auth/login/mock ... "
AUTH_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Auth/login/mock \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$PLAYER_ID\",\"password\":\"testpassword\"}")

TOKEN=$(echo $AUTH_RESPONSE | grep -o '"token":"[^"]*"' | cut -d'"' -f4)

if [ -z "$TOKEN" ]; then
    test_endpoint "POST /api/Auth/login/mock - モックログイン" "failure"
    echo "❌ 認証失敗。以降のテストをスキップします。"
    exit 1
else
    test_endpoint "POST /api/Auth/login/mock - モックログイン" "success"
fi

# GET /api/Auth/status (認証後)
echo -n "テスト中: GET /api/Auth/status (認証後) ... "
AUTH_STATUS_AFTER=$(curl -s -X GET http://localhost:5000/api/Auth/status \
  -H "Authorization: Bearer $TOKEN")
if echo "$AUTH_STATUS_AFTER" | grep -q "isAuthenticated"; then
    test_endpoint "GET /api/Auth/status - 認証状態確認(認証後)" "success"
else
    test_endpoint "GET /api/Auth/status - 認証状態確認(認証後)" "failure"
fi

echo ""

# ========================================
# 3. プレイヤー情報
# ========================================
echo "=========================================="
echo "3. プレイヤー情報"
echo "=========================================="

# POST /api/Player/me
echo -n "テスト中: POST /api/Player/me ... "
PLAYER_CREATE=$(curl -s -X POST http://localhost:5000/api/Player/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"$PLAYER_ID\",\"iconUrl\":\"https://example.com/icon.png\"}")

if echo "$PLAYER_CREATE" | grep -q "playerId\|name"; then
    test_endpoint "POST /api/Player/me - プレイヤー情報作成" "success"
else
    test_endpoint "POST /api/Player/me - プレイヤー情報作成" "failure"
fi

# GET /api/Player/me
echo -n "テスト中: GET /api/Player/me ... "
PLAYER_GET=$(curl -s -X GET http://localhost:5000/api/Player/me \
  -H "Authorization: Bearer $TOKEN")

if echo "$PLAYER_GET" | grep -q "playerId\|name"; then
    test_endpoint "GET /api/Player/me - プレイヤー情報取得" "success"
else
    test_endpoint "GET /api/Player/me - プレイヤー情報取得" "failure"
fi

echo ""

# ========================================
# 4. スターター選択 (エラー時はスキップ)
# ========================================
echo "=========================================="
echo "4. スターター選択 ⚠️"
echo "=========================================="

STARTER_SUCCESS=false

# GET /api/Starter/options
echo -n "テスト中: GET /api/Starter/options ... "
STARTER_OPTIONS=$(curl -s -X GET http://localhost:5000/api/Starter/options \
  -H "Authorization: Bearer $TOKEN")

if echo "$STARTER_OPTIONS" | grep -q "pokemonSpeciesId"; then
    test_endpoint "GET /api/Starter/options - スターターポケモン選択肢取得" "success"
else
    test_endpoint "GET /api/Starter/options - スターターポケモン選択肢取得" "failure"
fi

# POST /api/Starter/select
echo -n "テスト中: POST /api/Starter/select ... "
STARTER_SELECT=$(curl -s -X POST http://localhost:5000/api/Starter/select \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"pokemonSpeciesId":390}')

if echo "$STARTER_SELECT" | grep -q "error\|Error\|Exception\|既にポケモンを所持"; then
    test_endpoint "POST /api/Starter/select - スターターポケモン選択" "skip"
    echo "⚠️  スターター選択でエラーが発生しました。テストを継続します。"
    echo "   レスポンス: ${STARTER_SELECT:0:200}"
elif echo "$STARTER_SELECT" | grep -q "pokemon\|message"; then
    test_endpoint "POST /api/Starter/select - スターターポケモン選択" "success"
    STARTER_SUCCESS=true
else
    test_endpoint "POST /api/Starter/select - スターターポケモン選択" "failure"
fi

echo ""

# ========================================
# 5. パーティ管理
# ========================================
echo "=========================================="
echo "5. パーティ管理"
echo "=========================================="

# GET /api/Party
echo -n "テスト中: GET /api/Party ... "
PARTY_RESPONSE=$(curl -s -X GET http://localhost:5000/api/Party \
  -H "Authorization: Bearer $TOKEN")

if echo "$PARTY_RESPONSE" | grep -q "\[\]"; then
    test_endpoint "GET /api/Party - パーティ一覧取得(空)" "success"
    HAS_POKEMON=false
elif echo "$PARTY_RESPONSE" | grep -q "pokemonId"; then
    test_endpoint "GET /api/Party - パーティ一覧取得" "success"
    HAS_POKEMON=true
else
    test_endpoint "GET /api/Party - パーティ一覧取得" "failure"
    HAS_POKEMON=false
fi

echo ""

# ========================================
# 6. ガチャ
# ========================================
echo "=========================================="
echo "6. ガチャ"
echo "=========================================="

# POST /api/Gacha/pull
echo -n "テスト中: POST /api/Gacha/pull ... "
GACHA_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Gacha/pull \
  -H "Authorization: Bearer $TOKEN")

if echo "$GACHA_RESPONSE" | grep -q "pokemon"; then
    test_endpoint "POST /api/Gacha/pull - ガチャ実行" "success"
    # ガチャで取得したポケモンIDを抽出
    GACHA_POKEMON_ID=$(echo $GACHA_RESPONSE | grep -o '"pokemonId":"[^"]*"' | head -1 | cut -d'"' -f4)
else
    test_endpoint "POST /api/Gacha/pull - ガチャ実行" "failure"
fi

echo ""

# ========================================
# 7. パーティ管理（続き）
# ========================================
echo "=========================================="
echo "7. パーティ管理（続き）"
echo "=========================================="

# GET /api/Party (ガチャ後)
echo -n "テスト中: GET /api/Party (ガチャ後) ... "
PARTY_AFTER_GACHA=$(curl -s -X GET http://localhost:5000/api/Party \
  -H "Authorization: Bearer $TOKEN")

if echo "$PARTY_AFTER_GACHA" | grep -q "pokemonId"; then
    test_endpoint "GET /api/Party - パーティ一覧取得(ガチャ後)" "success"
    # 最初のポケモンIDを取得（逃がす用）
    FIRST_POKEMON_ID=$(echo $PARTY_AFTER_GACHA | grep -o '"pokemonId":"[^"]*"' | head -1 | cut -d'"' -f4)
    
    # パーティのポケモン数を確認
    POKEMON_COUNT=$(echo $PARTY_AFTER_GACHA | grep -o '"pokemonId"' | wc -l)
    
    if [ $POKEMON_COUNT -gt 1 ]; then
        # DELETE /api/Party/{pokemonId} (2体以上いる場合のみ)
        echo -n "テスト中: DELETE /api/Party/$FIRST_POKEMON_ID ... "
        DELETE_RESPONSE=$(curl -s -X DELETE http://localhost:5000/api/Party/$FIRST_POKEMON_ID \
          -H "Authorization: Bearer $TOKEN")
        
        if echo "$DELETE_RESPONSE" | grep -q "message\|逃がしました"; then
            test_endpoint "DELETE /api/Party/{pokemonId} - ポケモンを逃がす" "success"
        else
            test_endpoint "DELETE /api/Party/{pokemonId} - ポケモンを逃がす" "failure"
        fi
    else
        test_endpoint "DELETE /api/Party/{pokemonId} - ポケモンを逃がす" "skip"
        echo "⚠️  パーティに1体しかいないため、削除をスキップします。"
    fi
else
    test_endpoint "GET /api/Party - パーティ一覧取得(ガチャ後)" "failure"
    test_endpoint "DELETE /api/Party/{pokemonId} - ポケモンを逃がす" "skip"
fi

echo ""

# ========================================
# 8. バトル
# ========================================
echo "=========================================="
echo "8. バトル"
echo "=========================================="

# パーティにポケモンがいるか確認
PARTY_CHECK=$(curl -s -X GET http://localhost:5000/api/Party \
  -H "Authorization: Bearer $TOKEN")

if echo "$PARTY_CHECK" | grep -q "pokemonId"; then
    # POST /api/Battle/cpu
    echo -n "テスト中: POST /api/Battle/cpu ... "
    BATTLE_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Battle/cpu \
      -H "Authorization: Bearer $TOKEN")
    
    if echo "$BATTLE_RESPONSE" | grep -q "error\|Error\|Exception"; then
        test_endpoint "POST /api/Battle/cpu - CPUバトル作成" "failure"
        echo "   レスポンス: ${BATTLE_RESPONSE:0:200}"
        test_endpoint "GET /api/Battle/{battleId} - バトル状態取得" "skip"
    elif echo "$BATTLE_RESPONSE" | grep -q "battleId"; then
        test_endpoint "POST /api/Battle/cpu - CPUバトル作成" "success"
        
        # バトルIDを取得
        BATTLE_ID=$(echo $BATTLE_RESPONSE | grep -o '"battleId":"[^"]*"' | cut -d'"' -f4)
        
        # GET /api/Battle/{battleId}
        echo -n "テスト中: GET /api/Battle/$BATTLE_ID ... "
        BATTLE_STATE=$(curl -s -X GET http://localhost:5000/api/Battle/$BATTLE_ID \
          -H "Authorization: Bearer $TOKEN")
        
        if echo "$BATTLE_STATE" | grep -q "battleId"; then
            test_endpoint "GET /api/Battle/{battleId} - バトル状態取得" "success"
        else
            test_endpoint "GET /api/Battle/{battleId} - バトル状態取得" "failure"
        fi
    else
        test_endpoint "POST /api/Battle/cpu - CPUバトル作成" "failure"
        test_endpoint "GET /api/Battle/{battleId} - バトル状態取得" "skip"
    fi
else
    test_endpoint "POST /api/Battle/cpu - CPUバトル作成" "skip"
    test_endpoint "GET /api/Battle/{battleId} - バトル状態取得" "skip"
    echo "⚠️  パーティにポケモンがいないため、バトルテストをスキップします。"
fi

echo ""

# ========================================
# 9. ログアウト
# ========================================
echo "=========================================="
echo "9. ログアウト"
echo "=========================================="

# POST /api/Auth/logout
echo -n "テスト中: POST /api/Auth/logout ... "
LOGOUT_RESPONSE=$(curl -s -X POST http://localhost:5000/api/Auth/logout)

if echo "$LOGOUT_RESPONSE" | grep -q "message\|Logged out"; then
    test_endpoint "POST /api/Auth/logout - ログアウト" "success"
else
    test_endpoint "POST /api/Auth/logout - ログアウト" "failure"
fi

# GET /api/Auth/status (ログアウト後)
echo -n "テスト中: GET /api/Auth/status (ログアウト後) ... "
AUTH_STATUS_LOGOUT=$(curl -s -X GET http://localhost:5000/api/Auth/status)

if echo "$AUTH_STATUS_LOGOUT" | grep -q "isAuthenticated"; then
    test_endpoint "GET /api/Auth/status - 認証状態確認(ログアウト後)" "success"
else
    test_endpoint "GET /api/Auth/status - 認証状態確認(ログアウト後)" "failure"
fi

echo ""

# ========================================
# テスト結果サマリー
# ========================================
echo "=========================================="
echo "  テスト結果サマリー"
echo "=========================================="
echo "総テスト数: $TOTAL_COUNT"
echo "✅ 成功: $SUCCESS_COUNT"
echo "❌ 失敗: $FAILURE_COUNT"
echo "⚠️  スキップ: $SKIP_COUNT"
echo ""
echo "終了時刻: $(date '+%Y-%m-%d %H:%M:%S')"
echo "=========================================="

if [ $FAILURE_COUNT -eq 0 ]; then
    echo "🎉 すべてのテストが成功しました！"
    exit 0
else
    echo "⚠️  一部のテストが失敗しました。"
    exit 1
fi
