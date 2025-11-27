# API テスト方法

---

## 1. プレイヤー登録

### エンドポイント(Player)

```http
POST /api/players
```

### リクエスト例(Player)

```bash
curl -X POST http://localhost:5000/api/players \
  -H "Content-Type: application/json" \
  -d '{"name":"テストユーザー","iconUrl":"https://example.com/icon.png"}'
```

## 2. ポケモン登録

### エンドポイント(Party)

```http
POST /api/players/{playerId}/party
```

### curl 例(Party)

```bash
curl -X POST http://localhost:5000/api/players/example-player-id/party \
  -H "Content-Type: application/json" \
  -d '{"speciesId":1, "level":5}'
```

---

## テスト後の確認

### プレイヤー情報取得

```bash
curl http://localhost:5000/api/players/example-player-id
```

### パーティ一覧取得

```bash
curl http://localhost:5000/api/players/example-player-id/party
```

## その他のエンドポイント

その他のエンドポイントは Swagger UI で確認。

```bash
http://localhost:5000/swagger/index.html
```
