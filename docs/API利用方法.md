# APIテスト方法

---

## 0. 認証トークン作成

```bash
# JWTトークンの発行
dotnet run --project tools/jwt-generator/jwt-generator.csproj -- "your-very-strong-secret-key-change-this-minimum-32-characters" "example-player-id"
```

## 1. プレイヤー登録

### エンドポイント

```http
POST /api/players
```

### リクエスト例

```json
{
  "name": "テストユーザー",
  "iconUrl": "https://example.com/icon.png"
}
```

**注意**: `playerId`はJWTトークンの`sub`/`oid`/`nameidentifier`クレームから自動的に取得されます。リクエストボディに含めないでください。

### curl例

```bash
curl -X POST http://localhost:5000/api/players \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"name":"テストユーザー","iconUrl":"https://example.com/icon.png"}'
```

## 2. ポケモン登録

### エンドポイント

```http
POST /api/players/{playerId}/party
```

### リクエスト例

```json
{
  "speciesId": 1,
  "level": 5
}
```

### curl例

```bash
curl -X POST http://localhost:5000/api/players/example-player-id/party \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"speciesId":1, "level":5}'
```  
---

## テスト後の確認

### プレイヤー情報取得

```bash
curl -H "Authorization: Bearer <token>" \
  http://localhost:5000/api/players/example-player-id
```

### パーティ一覧取得

```bash
curl -H "Authorization: Bearer <token>" \
  http://localhost:5000/api/players/example-player-id/party
```

## その他のエンドポイント

その他のエンドポイントはSwagger UIで確認。

```bash
http://localhost:5000/swagger/index.html
```
