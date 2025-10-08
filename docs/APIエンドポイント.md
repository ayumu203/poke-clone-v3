# APIエンドポイント

## ポケモン種族データ

### GET /api/pokemon-species

ポケモン種族一覧取得

- `skip` (int): スキップする件数 (default: 0)
- `take` (int): 取得する件数 (default: 20)

**例**:

```bash
curl http://localhost:5185/api/pokemon-species?skip=0&take=10
```

特定のポケモン種族取得

**例**:

```bash
curl http://localhost:5185/api/pokemon-species/1
```

## 技データ

### GET /api/moves

技一覧取得

**クエリパラメータ**:

- `skip` (int): スキップする件数 (default: 0)
- `take` (int): 取得する件数 (default: 20)

**例**:

```bash
curl http://localhost:5185/api/moves?skip=0&take=10
```

特定の技取得

**例**:
```bash
curl http://localhost:5185/api/moves/1
```