# Poke-Clone バックエンドコードレビュー (Gemini)

レビュー日: 2025-12-02

Claudeのレビューに加え、よりアーキテクチャ、整合性、セキュリティの観点から深くコードを分析しました。

---

## 1. Server.Domain レビュー

### 1.1 ドメインモデルの貧血と責務の配置

**指摘事項: `Battle.cs` におけるステータス再計算**

`Battle.cs` の `ProcessMoveAction` メソッド内で、回復量の計算のために `_statCalculator.CalcHp` を呼び出しています (行236)。
また、`ProcessTurn` でも素早さを計算しています (行36-37)。

```csharp
var maxHp = _statCalculator.CalcHp(attackerPokemon.Level, attackerPokemon.Species.BaseHp);
```

**問題点**:
戦闘中のポケモン (`PokemonState` や `BattlePlayer` 内のポケモン) が、自身の「最大HP」や「現在の素早さ」といった派生ステータスを保持していない、あるいは `Battle` クラスがそれを知らない状態です。
これにより、`Battle` クラスが「戦闘ロジック」だけでなく「ステータス計算のオーケストレーション」まで担ってしまっています。

**提案**:
戦闘開始時にステータスを確定させた `BattlePokemon` (仮) のようなリッチなモデルを作成し、`Battle` クラスはそのモデルのプロパティ（`MaxHp`, `Speed`など）を直接参照するようにすべきです。計算は `Battle` クラスの外（初期化時）で行うべきです。

### 1.2 テスタビリティとランダム性

**指摘事項: `Random` クラスの直接使用**

`Battle.cs` や `CPUBattlePlayer.cs` で `new Random()` が直接使用されています。

**問題点**:
ユニットテストにおいて、確率が絡むロジック（命中、急所、追加効果、CPUの行動）のテストが困難です。特定の乱数シードを固定できないため、再現性のあるテストが書けません。

**提案**:
`IRandomProvider` インターフェースを導入し、DIしてください。テスト時は固定値を返すモックを使用することで、戦闘ロジックの完全なテストが可能になります。
クラス図への追記指示も忘れない(/mnt/c/Users/cs23017/Shizuoka University/ドキュメント/dev/01_poke_clone-v3/Docs/UML/クラス図更新手順.md).

---

## 2. Server.Application レビュー

### 2.1 データ整合性と競合状態 (Race Condition)

**指摘事項: `Player.Money` の更新における競合リスク**

`BattleService.ProcessPostBattleAsync` (行388) で戦闘勝利時にお金を加算しています。
一方で、`GachaController` (WebAPI層ですがロジックが含まれています) でもお金を減算しています。

**問題点**:
ユーザーが「戦闘終了」と「ガチャ実行」をほぼ同時に行った場合、`Player` エンティティの読み込みと書き込みの間に競合が発生し、所持金の整合性が崩れる可能性があります（例：戦闘報酬が上書きされる、あるいはガチャ代金が引かれないままになる）。
現在の `BattleService` は Redis ロックを使っていますが、これは「バトルID」に対するロックであり、「プレイヤーID」に対するロックではありません。

**提案**:
1. **楽観的排他制御**: `Player` テーブルに `RowVersion` カラムを追加し、更新時に競合を検知してリトライさせる。
2. **悲観的排他制御**: 重要な資産（Money）の更新時は、データベースレベルで行ロックをかけるか、プレイヤー単位の分散ロックを導入する。

### 2.2 情報漏洩のリスク

**指摘事項: `GetBattleStateAsync` の戻り値**

`BattleService` は `BattleState` をそのまま返しています。これには対戦相手の `PokemonEntities` や `Moves` がすべて含まれています。

**問題点**:
APIレスポンスとしてこれをそのままクライアントに返すと、対戦相手の「選んでいない技」や「正確な残りHP数値」などが、ブラウザの開発者ツール等で見えてしまう可能性があります（チートの温床）。

**提案**:
クライアントに返すための `BattleViewDto` を作成し、プレイヤー視点で見えてはいけない情報をマスク（隠蔽）して返す仕組みが必要です。

---

## 3. Server.Infrastructure レビュー

### 3.1 パフォーマンスとN+1問題

**指摘事項: `PokemonRepository.GetPlayerPartyAsync` の実装**

```csharp
foreach (var pokemon in playerParty.Party)
{
    await _context.Entry(pokemon).Reference(p => p.Species).LoadAsync();
    await _context.Entry(pokemon).Collection(p => p.Moves).LoadAsync();
}
```

**問題点**:
パーティのポケモン数分だけ追加のクエリが発行されます（N+1問題）。パーティは最大6匹なので致命的ではありませんが、DB負荷の観点からは推奨されません。

**提案**:
`Include` / `ThenInclude` を使用して、1回のクエリで取得するように修正してください。

```csharp
var playerParty = await _context.PlayerParties
    .Include(pp => pp.Party)
        .ThenInclude(p => p.Species)
    .Include(pp => pp.Party)
        .ThenInclude(p => p.Moves)
    .FirstOrDefaultAsync(...);
```

### 3.2 不要なデータロード

**指摘事項: `IsPartyFullAsync` の実装**

```csharp
var playerParty = await _context.PlayerParties
    .Include(pp => pp.Party) // パーティの中身を全部ロードしている
    .FirstOrDefaultAsync(...);
return playerParty.Party.Count >= 6;
```

**問題点**:
単に「数」を知りたいだけなのに、ポケモンのエンティティ情報をすべてDBから取得しています。メモリと帯域の無駄です。

**提案**:
Countのみを取得するクエリにするか、PlayerPartyテーブルに `Count` カラムを持たせる（非正規化）などを検討してください。
EF Coreであれば以下のように書けます：

```csharp
var count = await _context.Entry(playerParty)
    .Collection(p => p.Party)
    .Query()
    .CountAsync();
```
※ ただし `playerParty` がAttachされている必要があります。単純なクエリなら：
```csharp
var count = await _context.Pokemons
    .Where(p => p.PlayerParty.PlayerId == playerId)
    .CountAsync();
```
（スキーマ構造によりますが、直接カウントする方が軽量です）

---

## 4. Server.WebAPI レビュー

### 4.1 アーキテクチャ違反（Fat Controller）

**指摘事項: コントローラーへのビジネスロジック混入**

`GachaController`, `StarterController`, `PartyController` に、本来 Application 層にあるべきビジネスロジックが直書きされています。

- **GachaController**: ガチャの抽選ロジック、所持金チェック、トランザクション管理（未実装だが本来必要）
- **StarterController**: スターターの定義、生成ロジック
- **PartyController**: 「最後の1匹は逃がせない」というドメインルール

**問題点**:
WebAPI層が肥大化し、ロジックの再利用性が低下しています。また、テストもコントローラー経由で行う必要があり複雑になります。

**提案**:
これらのロジックを `GachaService`, `PlayerService` (あるいは `PartyService`) に移動し、コントローラーは「HTTPリクエストの受け付けと、サービスへの委譲」に徹するべきです。

### 4.2 トランザクション管理の欠如

**指摘事項: `GachaController.PullGacha`**

```csharp
player.Money -= GachaCost;
await _playerRepository.UpdateAsync(player);
// ここでエラーが起きたら？
await _pokemonRepository.AddToPartyAsync(playerId, pokemon);
```

**問題点**:
お金を減らした後、ポケモンの保存に失敗すると「お金だけ減ってポケモンが手に入らない」という状態になります。

**提案**:
`UnitOfWork` パターンを導入するか、明示的に `IDbContextTransaction` を使用して、操作のアトミック性を保証してください。これは Application 層の Service で行うべき責務です。

### 4.3 HTTPステータスコード

**指摘事項: `PartyController.GetParty`**

```csharp
if (string.IsNullOrEmpty(playerId))
{
    return Unauthorized();
}
```

**提案**:
`[Authorize]` 属性がついている場合、通常 `User.Identity.Name` は設定されているはずです。もしこれが null ならば、認証ミドルウェアの設定ミスか、予期せぬ状態です。`500 Internal Server Error` または `400 Bad Request` が適切かもしれません。`401 Unauthorized` は「認証されていない」ことを示しますが、ここは認証済みのコンテキスト内での異常系です。

---

## 総合まとめ

コードは全体的に読みやすく、Clean Architecture の構造を意識して作られていますが、**「戦闘以外の機能（ガチャ、パーティ管理など）」の実装が WebAPI 層に漏れ出している** 点が最大のアーキテクチャ上の課題です。

また、**データ整合性（トランザクション、競合）** と **セキュリティ（情報隠蔽）** の観点で、本番運用するにはいくつかの修正が必要です。

### 推奨アクション（優先度順）

1.  **トランザクション管理の導入**: 特にガチャ機能におけるお金とポケモンの整合性確保。
2.  **Application Service の拡充**: コントローラーからロジックを剥がし、`GachaService` 等へ移動。
3.  **N+1問題の修正**: Repository のクエリ最適化。
4.  **ドメインモデルの強化**: `Battle` クラスの計算責務を減らし、モデルにデータを持たせる。
