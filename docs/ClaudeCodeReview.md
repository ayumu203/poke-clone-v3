# コードレビュー結果

**作成日**: 2025-12-05  
**レビュー対象**: Server.Domain, Server.Application, Server.WebAPI  
**レビュアー**: Claude (Antigravity)

---

## 目次

1. [Server.Domain層](#serverdomain層)
2. [Server.Application層](#serverapplication層)
3. [Server.WebAPI層](#serverwebapi層)
4. [全体的な改善提案](#全体的な改善提案)
5. [優先度付きアクションプラン](#優先度付きアクションプラン)

---

## Server.Domain層

### 1. Battle.cs

#### 🔴 高優先度

##### 1.1 `Random`インスタンスの不適切な使用 (行110, 170, 183, 293)

**問題**:
```csharp
var random = new Random();
```

メソッド内で毎回`new Random()`を作成すると、短時間に複数回呼び出された場合、同じシード値が使われて同じ乱数列が生成される可能性があります。

**修正案**:
```csharp
// Battleクラスのフィールドとして定義
private static readonly Random _random = new Random();

// または依存注目で注入
private readonly IRandom _random;
```

##### 1.2 マジックナンバーの使用 (行162-168)

**問題**:
```csharp
int critChanceDenominator = move.CritRate switch
{
    0 => 16,
    1 => 8,
    2 => 2,
    _ => 1
};
```

**修正案**:
```csharp
// 定数として定義
private static class CriticalRates
{
    public const int Stage0 = 16;
    public const int Stage1 = 8;
    public const int Stage2 = 2;
    public const int Stage3Plus = 1;
}
```

##### 1.3 TODOコメントの放置 (行90)

**問題**:
```csharp
//TODO: 状態異常・ステータス上昇技の処理の追加
```

**対応**: すでに実装済みのため、コメントを削除するか更新する。

#### 🟡 中優先度

##### 1.4 状態異常判定の簡易実装 (行181-182)

**問題**:
```csharp
// AilmentChanceが0の場合は50%とみなす（簡易実装）
var chance = move.AilmentChance == 0 ? 50 : move.AilmentChance;
```

**改善案**: PokeAPIのデータ仕様を確認し、`AilmentChance = 0`の意味を明確にする。通常は「必中」か「発動なし」のはず。

##### 1.5 ステータス変化の対象判定ロジックが簡易的 (行197-217)

**問題**:
```csharp
// 対象判定（簡易ロジック）
// 変化技で上昇 -> 自分
// それ以外 -> 相手
bool isSelfTarget = move.DamageClass == Enums.DamageClass.Status && change.Change > 0;
```

**改善案**: PokeAPIの`move_meta.stat_chance`フィールドを活用し、確率発動の技に対応する。現在は`Program.cs`で`StatChance`を取得していないため、データ取得スクリプトの修正が必要。

##### 1.6 HP回復計算の課題 (行220-238)

**問題**: コメントで指摘されている通り、`MaxHp`の計算が暫定的。

**現在の実装**:
```csharp
var maxHp = _statCalculator.CalcHp(attackerPokemon.Level, attackerPokemon.Species.BaseHp);
healing = (int)(maxHp * (move.Healing / 100.0));
```

**改善案**: IV・EV値を考慮した正確な`MaxHp`計算。ただし、現在のMVPスコープでは許容範囲。

#### 🟢 低優先度

##### 1.7 エラーメッセージの国際化対応 (行104, 146)

**問題**: ハードコードされた英語メッセージ。

**改善案**: リソースファイルを使用した多言語対応。

---

### 2. ProcessResult.cs

#### 🟡 中優先度

##### 2.1 `HitContext`のnull許容設計

**問題**: `HitContext?`として定義されているが、攻撃が成功した場合は必ず存在すべき。

**改善案**:
```csharp
public class MoveResult
{
    // ...
    public bool IsSuccess { get; set; }
    
    // IsSuccess = true の場合のみ使用
    public HitContext? HitContext { get; set; }
}
```

明示的な検証を追加:
```csharp
if (IsSuccess && HitContext == null)
{
    throw new InvalidOperationException("Successful attack must have HitContext");
}
```

---

### 3. PlayerParty.cs

#### 🔴 高優先度

##### 3.1 `PlayerPartyId`の冗長性

**問題**: レビュー回答で指摘された通り、1プレイヤー=1パーティの設計では`PlayerPartyId`は不要。

**現状**:
```csharp
public class PlayerParty
{
    public int PlayerPartyId { get; set; }  // 不要
    public string PlayerId { get; set; } = string.Empty;
    public List<Pokemon> Party { get; set; } = new();
}
```

**修正案**:
```csharp
public class PlayerParty
{
    public string PlayerId { get; set; } = string.Empty;  // プライマリキーに変更
    public List<Pokemon> Party { get; set; } = new();
}

// AppDbContext.cs
entity.HasKey(pp => pp.PlayerId);
```

---

### 4. PokemonStat.cs (Enums/PokemonStat.cs)

#### 🔴 高優先度

##### 4.1 `Hp`の不適切な定義

**問題**: レビュー回答で指摘された通り、HPはランク補正の対象外。

**現状**:
```csharp
public enum PokemonStat
{
    Hp,  // ← 削除すべき
    Attack,
    Defense,
    // ...
}
```

**修正案**: `Hp`を削除し、`Program.cs`のデータ取得スクリプトでHP関連の`stat_changes`が除外されていることを確認。

---

##  Server.Application層

### 5. BattleService.cs

#### 🔴 高優先度

##### 5.1 `_pendingActions`の非スレッドセーフな使用懸念

**場所**: 実際には`BattleHub.cs`に存在。後述。

##### 5.2 例外処理の不備 (行45-67)

**問題**: `CreateBattleAsync`でプレイヤーが見つからない場合に汎用的な例外をスロー。

**現状**:
```csharp
if (player1 == null || player2 == null)
{
    throw new InvalidOperationException("Player not found");
}
```

**改善案**: カスタム例外を使用。
```csharp
public class PlayerNotFoundException : Exception
{
    public string PlayerId { get; }
    public PlayerNotFoundException(string playerId) 
        : base($"Player '{playerId}' not found")
    {
        PlayerId = playerId;
    }
}

if (player1 == null)
{
    throw new PlayerNotFoundException(player1Id);
}
```

##### 5.3 依存関係の多さ (行21-43)

**問題**: 10個の依存関係を持つコンストラクタ。

**改善案**: 関連サービスをグループ化。
```csharp
public interface IBattleContext
{
    IDamageCalculator DamageCalculator { get; }
    ITypeEffectivenessManager TypeEffectivenessManager { get; }
    IStatCalculator StatCalculator { get; }
}

public BattleService(
    IBattleContext battleContext,
    IPlayerRepository playerRepository,
    // ...
)
```

#### 🟡 中優先度

##### 5.4 CPUポケモン生成の簡易実装 (行93-107)

**問題**: 常に最初の種族を使用。

**改善案**: ランダムまたはレベルベースの選択。
```csharp
var random = new Random();
var wildSpecies = allSpecies.ElementAt(random.Next(allSpecies.Count()));
```

##### 5.5 ハードコードされたタイムアウト値 (行128)

**問題**:
```csharp
var lockAcquired = await _battleRepository.TryLockAsync(battleId, TimeSpan.FromSeconds(10));
```

**改善案**: 設定ファイルから読み込み。
```csharp
private readonly AppSettings _settings;

var lockAcquired = await _battleRepository.TryLockAsync(
    battleId, 
    TimeSpan.FromSeconds(_settings.BattleLockTimeoutSeconds)
);
```

##### 5.6 プレイヤーIDの比較ロジックが冗長 (行154-159)

**問題**:
```csharp
var p1Action = action1.PlayerId == battleState.Player1.PlayerId ? action1 : action2;
var p2Action = action1.PlayerId == battleState.Player2.PlayerId ? action1 : action2;

if (p1Action.PlayerId != battleState.Player1.PlayerId) p1Action = action1;
if (p2Action.PlayerId != battleState.Player2.PlayerId) p2Action = action2;
```

**改善案**:
```csharp
var (p1Action, p2Action) = action1.PlayerId == battleState.Player1.PlayerId
    ? (action1, action2)
    : (action2, action1);
```

#### 🟢 低優先度

##### 5.7 CPU判定のマジック文字列 (行308, 369)

**問題**:
```csharp
bool isCpuBattle = opponent.Player.PlayerId == "CPU";
```

**改善案**:
```csharp
public static class SystemPlayers
{
    public const string CpuPlayerId = "CPU";
}
```

---

## Server.WebAPI層

### 6. BattleHub.cs

#### 🔴 高優先度

##### 6.1 静的フィールド`_pendingActions`のスレッドセーフ性 (行10)

**問題**:
```csharp
private static readonly Dictionary<string, List<PlayerAction>> _pendingActions = new();
```

複数のクライアントから同時にアクセスされる可能性があり、`lock`を使用しているものの設計が脆弱。

**改善案**: `ConcurrentDictionary`を使用。
```csharp
private static readonly ConcurrentDictionary<string, List<PlayerAction>> _pendingActions = new();
```

また、アクションの追加ロジックを改善:
```csharp
_pendingActions.AddOrUpdate(
    battleId,
    new List<PlayerAction> { action },
    (key, existingList) =>
    {
        if (!existingList.Any(a => a.PlayerId == action.PlayerId))
        {
            existingList.Add(action);
        }
        return existingList;
    }
);
```

##### 6.2 CPU AIの簡易実装 (行48-78)

**問題**: ランダムに技を選択するのみ。

**改善案**: より高度なAIロジック。
```csharp
// 戦略的な技選択
private PlayerAction GenerateCpuAction(PlayerState cpuPlayer, PlayerState opponent)
{
    var activePokemon = cpuPlayer.PokemonEntities[cpuPlayer.ActivePokemonIndex];
    var opponentPokemon = opponent.PokemonEntities[opponent.ActivePokemonIndex];
    
    // 1. 効果抜群の技を優先
    // 2. PP残量を考慮
    // 3. HP回復技の判断
    // ...
    
    return new PlayerAction { /* ... */ };
}
```

##### 6.3 エラーハンドリングの不足 (行94-139)

**問題**: `ProcessTurn`内の`try-catch`でエラーをコンソール出力するのみ。

**改善案**: 構造化ロギング。
```csharp
try
{
    // ...
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error processing turn for battle {BattleId}", battleId);
    await Clients.Group(battleId).SendAsync("TurnProcessingFailed", new 
    { 
        ErrorCode = "TURN_PROCESSING_ERROR",
        Message = "An error occurred while processing the turn"
    });
}
```

#### 🟡 中優先度

##### 6.4 行動待機状態の永続化なし

**問題**: `_pendingActions`はインメモリのため、サーバー再起動で失われる。

**改善案**: Redisに保存。
```csharp
private readonly IDistributedCache _cache;

// アクション保存
await _cache.SetStringAsync(
    $"battle:{battleId}:pending_actions",
    JsonSerializer.Serialize(actionsToProcess),
    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }
);
```

---

### 7. AuthController.cs

#### 🟡 中優先度

##### 7.1 モック認証の本番環境への混入リスク (行36-68)

**問題**: `/login/mock`エンドポイントが本番環境で利用可能。

**改善案**: 環境変数による制御。
```csharp
[HttpPost("login/mock")]
public IActionResult MockLogin([FromBody] MockLoginRequest request)
{
    if (!_environment.IsDevelopment())
    {
        return NotFound();
    }
    // ...
}
```

##### 7.2 JWTトークンの有効期限が固定 (行54)

**問題**:
```csharp
expires: DateTime.Now.AddDays(1),
```

**改善案**: 設定ファイルから読み込み。
```csharp
expires: DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
```

---

### 8. BattleController.cs

#### 🟢 低優先度

##### 8.1 対人バトル作成エンドポイントの欠如

**問題**: `POST /api/Battle/create`エンドポイントが実装されていない。

**改善案**: 必要に応じて実装、または不要であればドキュメントから削除。

---

## 全体的な改善提案

### 9. ロギング

#### 🔴 高優先度

##### 9.1 構造化ロギングの導入

**現状**: `Console.WriteLine`を使用 (`BattleHub.cs`行137)。

**改善案**: `ILogger`を使用。
```csharp
private readonly ILogger<BattleHub> _logger;

// ...

_logger.LogError(ex, "Error in ProcessTurn for battle {BattleId}", battleId);
```

---

### 10. バリデーション

#### 🟡 中優先度

##### 10.1 DTOのバリデーション不足

**改善案**: FluentValidationを導入。
```csharp
public class PlayerActionValidator : AbstractValidator<PlayerAction>
{
    public PlayerActionValidator()
    {
        RuleFor(x => x.PlayerId).NotEmpty();
        RuleFor(x => x.ActionType).IsInEnum();
        RuleFor(x => x.MoveId).GreaterThan(0).When(x => x.ActionType == ActionType.Attack);
    }
}
```

---

### 11. セキュリティ

#### 🔴 高優先度

##### 11.1 認可チェックの不足

**問題**: `BattleHub.SubmitAction`でプレイヤーIDの検証がない。

**改善案**:
```csharp
public async Task SubmitAction(string battleId, PlayerAction action)
{
    var currentPlayerId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (action.PlayerId != currentPlayerId)
    {
        throw new UnauthorizedAccessException("Cannot submit action for another player");
    }
    // ...
}
```

---

### 12. テスト可能性

#### 🟡 中優先度

##### 12.1 静的フィールドの使用によるテストの困難さ

**問題**: `BattleHub._pendingActions`が静的フィールド。

**改善案**: 依存注入可能なサービスに分離。
```csharp
public interface IPendingActionsManager
{
    void AddAction(string battleId, PlayerAction action);
    List<PlayerAction>? TryGetAndRemoveActions(string battleId);
}
```

---

## 優先度付きアクションプラン

### 🔴 即座に対応すべき項目

1. **`Random`インスタンスの適切な管理** (Battle.cs)
2. **`PlayerParty`のプライマリキー変更** (PlayerParty.cs)
3. **`PokemonStat`から`Hp`を削除** (Enums/PokemonStat.cs)
4. **`_pendingActions`の`ConcurrentDictionary`化** (BattleHub.cs)
5. **認可チェックの追加** (BattleHub.cs)
6. **構造化ロギングの導入** (全体)

### 🟡 近日中に対応すべき項目

1. **カスタム例外の導入** (BattleService.cs)
2. **状態異常判定ロジックの改善** (Battle.cs)
3. **CPU AI戦略の改善** (BattleHub.cs)
4. **モック認証の環境制御** (AuthController.cs)
5. **DTOバリデーションの導入** (全体)

### 🟢 将来的に検討すべき項目

1. **国際化対応** (Battle.cs)
2. **CPU判定のマジック文字列削除** (BattleService.cs)
3. **対人バトル作成エンドポイント** (BattleController.cs)
4. **IV・EV値システムの実装** (StatCalculator)

---

## まとめ

全体的にコードの品質は良好ですが、以下の点で改善の余地があります:

1. **スレッドセーフ性**: 静的フィールドの使用を見直し、並行処理に対応
2. **エラーハンドリング**: カスタム例外と構造化ロギングの導入
3. **セキュリティ**: 認可チェックの強化
4. **テスト可能性**: 依存注入の改善、静的フィールドの削減
5. **保守性**: マジックナンバー・文字列の定数化

優先度の高い項目から順に対応することで、システムの堅牢性と保守性を大幅に向上できます。
