echo "Azure AD開発用トークン取得スクリプト"

# 設定値（実際の値に置き換えてください）
TENANT_ID="your-tenant-id"
CLIENT_ID="your-client-id"
CLIENT_SECRET="your-client-secret"
SCOPE="api://your-api-client-id/.default"

# トークン取得
response=$(curl -s -X POST "https://login.microsoftonline.com/$TENANT_ID/oauth2/v2.0/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=$CLIENT_ID&client_secret=$CLIENT_SECRET&scope=$SCOPE")

# JWTトークンを抽出
token=$(echo $response | jq -r '.access_token')

if [ "$token" != "null" ]; then
    echo "取得したJWTトークン:"
    echo "$token"
    echo ""
    echo "APIテスト用のcurlコマンド例:"
    echo "curl -H \"Authorization: Bearer $token\" http://localhost:5259/api/player/me"
else
    echo "トークンの取得に失敗しました:"
    echo $response
fi
