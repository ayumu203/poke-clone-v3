# Poke-Clone バックエンドコードレビュー

レビュー日: 2025-12-02

---

## 1. Server.Domain レビュー

### 1.1 Battle.cs

#### ⚠️ 改善点

**1. マジックナンバーの多用 (行110, 114, 162-168, 173, 183, 237, 244, 295)**

```csharp
// 現在
var random = new Random();
if (move.Accuracy > 0)
{
    var hitRoll = random.Next(1, 101); // 1-100の乱数
    if (hitRoll > move.Accuracy)
    {
        return new MoveResult { ... };
    }
}

int critChanceDenominator = move.CritRate switch
{
    0 => 16,
    1 => 8,
    2 => 2,
    _ => 1
};
```

**提案**: 定数として定義する

```csharp
private const int MinAccuracyRoll = 1;
private const int MaxAccuracyRoll = 100;
private const int DefaultAilmentChance = 100;
private const double CriticalDamageMultiplier = 1.5;

private static class CriticalHitChance
{
    public const int Stage0 = 16;
    public const int Stage1 = 8;
    public const int Stage2 = 2;
    public const int Stage3Plus = 1;
}
```

**2. 捕獲率の簡易実装 (行294-295)**

```csharp
var catchRate = random.Next(0, 100);
var isSuccess = catchRate < 50; // 50%の確率で捕獲成功（簡易版）
```

**提案**: 捕獲率計算を専用サービスに分離

```csharp
public interface ICaptureCalculator
{
    bool CalculateCaptureSuccess(Pokemon targetPokemon, int currentHp, int maxHp, Ailment? ailment = null);
}
```

**理由**: 
- 捕獲処理は独立したビジネスロジック
- 将来的にHP残量、状態異常、ボールの種類などを考慮する拡張が容易

**3. 手続き的な処理 (行236-237, 384)**

```csharp
// Battle.cs内でMaxHp計算
var maxHp = _statCalculator.CalcHp(attackerPokemon.Level, attackerPokemon.Species.BaseHp);
healing = (int)(maxHp * (move.Healing / 100.0));

// BattleService.cs内で報酬金額を直接計算
var moneyReward = loserPokemon.Level * 100;
```

**提案**: 計算ロジックを専用サービスに移動

```csharp
public interface IRewardCalculator
{
    int CalculateMoneyReward(int loserLevel);
}

public class RewardCalculator : IRewardCalculator
{
    private const int MoneyPerLevel = 100;
    
    public int CalculateMoneyReward(int loserLevel)
    {
        return loserLevel * MoneyPerLevel;
    }
}
```

**5. CPU判定のハードコーディング (行308)**

```csharp
bool isCpuBattle = opponent.Player.PlayerId == "CPU";
```

**提案**: 定数化または専用メソッド

```csharp
private const string CpuPlayerId = "CPU";

private bool IsCpuBattle(BattlePlayer player)
{
    return player.Player.PlayerId == CpuPlayerId;
}
```

### 1.2 Domain Entities

#### Player.cs

**⚠️ 改善点**

**マジックナンバー (行8)**

```csharp
public int Money { get; set; } = 3000;
```

**提案**:

```csharp
public const int InitialMoney = 3000;
public int Money { get; set; } = InitialMoney;
```

### 1.3 Domain Services

#### DamageCalculator.cs

**⚠️ 改善点**

**マジックナンバー (行43, 45)**

```csharp
var stab = (move.Type == attacker.Species.Type1 || ...) ? 1.5 : 1.0;
var baseDamage = ((2 * level / 5 + 2) * power * attackStat / defenseStat) / 50 + 2;
```

**提案**:

```csharp
private const double StabMultiplier = 1.5;
private const double NoStabMultiplier = 1.0;
private const int DamageFormulaConstant1 = 2;
private const int DamageFormulaConstant2 = 5;
private const int DamageFormulaConstant3 = 50;
```

#### TypeEffectivenessManager.cs

**⚠️ 改善点**

**可読性**: 相性テーブルが長い (162行)

**提案**: JSONファイルで管理し、起動時にロード

```json
{
  "Fire": {
    "Grass": 2.0,
    "Ice": 2.0,
    "Bug": 2.0,
    "Steel": 2.0,
    "Fire": 0.5,
    "Water": 0.5,
    "Rock": 0.5,
    "Dragon": 0.5
  }
}
```

---

## 2. Server.Application レビュー

### 2.1 BattleService.cs

#### ⚠️ 改善点

**1. 重複したコード (行162-202)**

**追記**
捕獲はCPU戦のみとなるように実装を修正.

```csharp
// Player1の捕獲チェック
if (p1Action.ActionType == Domain.Enums.ActionType.Catch)
{
    var isPartyFull = await _pokemonRepository.IsPartyFullAsync(battleState.Player1.PlayerId);
    if (isPartyFull)
    {
        var failedResult = new ProcessResult();
        failedResult.ActionResults.Add(new ActionResult { ... });
        return failedResult;
    }
}

// Player2の捕獲チェック (ほぼ同じコード)
if (p2Action.ActionType == Domain.Enums.ActionType.Catch)
{
    var isPartyFull = await _pokemonRepository.IsPartyFullAsync(battleState.Player2.PlayerId);
    // ...
}
```

**提案**: メソッド抽出

```csharp
private async Task<ProcessResult?> ValidateCatchAction(
    PlayerAction action, 
    PlayerState playerState)
{
    if (action.ActionType != Domain.Enums.ActionType.Catch)
        return null;
        
    var isPartyFull = await _pokemonRepository.IsPartyFullAsync(playerState.PlayerId);
    if (!isPartyFull)
        return null;
        
    return new ProcessResult
    {
        ActionResults = new List<ActionResult>
        {
            new ActionResult
            {
                ActionPokemonId = playerState.PokemonEntities[playerState.ActivePokemonIndex].PokemonId,
                ActionType = Domain.Enums.ActionType.Catch,
                CatchResult = new CatchResult { IsSuccess = false }
            }
        }
    };
}

// 使用例
var p1CatchValidation = await ValidateCatchAction(p1Action, battleState.Player1);
if (p1CatchValidation != null) return p1CatchValidation;

var p2CatchValidation = await ValidateCatchAction(p2Action, battleState.Player2);
if (p2CatchValidation != null) return p2CatchValidation;
```

**2. マジックナンバー (行384)**

```csharp
var moneyReward = loserPokemon.Level * 100;
```

**提案**: 前述の`IRewardCalculator`を使用

**3. CPU判定のハードコーディング (行369)**

```csharp
if (loserState.PlayerId != "CPU")
```

**提案**: 定数化

```csharp
private const string CpuPlayerId = "CPU";
```

**4. エラーハンドリングの不足**

- `GetEvolutionAsync`が`null`を返す可能性があるが、チェック後の処理が不十分 (行411)
- `GetByIdAsync`が`null`の場合の処理が早期リターンのみ (行324)

**提案**: ログ出力を追加

```csharp
if (battleState == null)
{
    _logger.LogWarning("Battle {BattleId} not found for post-battle processing", battleId);
    return;
}
```

### 2.2 CPUBattlePlayer.cs

#### ⚠️ 改善点

**1. バグ: Valueに誤った値を設定 (行29)**

```csharp
return new PlayerAction
{
    ActionType = ActionType.Attack,
    Value = moveIndex  // ❌ moveIndexではなくMoveIdを設定すべき
};
```

**修正**:

```csharp
var selectedMove = activePokemon.Moves[moveIndex];
return new PlayerAction
{
    ActionType = ActionType.Attack,
    Value = selectedMove.MoveId,
    PlayerId = cpuPlayer.Player.PlayerId
};
```

**2. PlayerId未設定**

`PlayerAction`に`PlayerId`プロパティがあるが設定されていない

---

## 3. Server.Infrastructure レビュー

### 3.1 PokemonRepository.cs

#### ⚠️ 改善点

**1. デバッグログの残存 (行27-60)**

```csharp
Console.WriteLine($"[DEBUG] GetPlayerPartyAsync called for playerId: {playerId}");
Console.WriteLine("[DEBUG] PlayerParty not found for playerId: " + playerId);
// ... 多数のConsole.WriteLine
```

**提案**: 本番環境では削除、または適切なロガーを使用

```csharp
_logger.LogDebug("GetPlayerPartyAsync called for playerId: {PlayerId}", playerId);
```

**2. N+1問題の可能性 (行49-58)**

```csharp
foreach (var pokemon in playerParty.Party)
{
    await _context.Entry(pokemon).Reference(p => p.Species).LoadAsync();
    await _context.Entry(pokemon).Collection(p => p.Moves).LoadAsync();
}
```

**提案**: Includeを使用

```csharp
var playerParty = await _context.PlayerParties
    .Include(pp => pp.Party)
        .ThenInclude(p => p.Species)
    .Include(pp => pp.Party)
        .ThenInclude(p => p.Moves)
    .FirstOrDefaultAsync(pp => pp.PlayerId == playerId);
```

**3. パフォーマンス: 不要なクエリ (行61, 122)**

```csharp
// GetPlayerPartyAsyncを2回呼ぶ箇所がある
var party = await _pokemonRepository.GetPlayerPartyAsync(playerId);
var pokemon = party?.FirstOrDefault(p => p.PokemonId == pokemonId);
```

**提案**: 直接クエリで取得

```csharp
var pokemon = await _context.Pokemons
    .Where(p => p.PokemonId == pokemonId)
    .FirstOrDefaultAsync();
```

### 3.2 PlayerRepository.cs

#### ⚠️ 改善点

**エラーハンドリング不足**

`UpdateAsync`や`DeleteAsync`で対象が見つからない場合の処理が不明確

**提案**:

```csharp
public async Task UpdateAsync(Player player)
{
    var existing = await GetByIdAsync(player.PlayerId);
    if (existing == null)
    {
        throw new InvalidOperationException($"Player {player.PlayerId} not found");
    }
    
    _context.Players.Update(player);
    await _context.SaveChangesAsync();
}
```

---

## 4. Server.WebAPI レビュー

### 4.1 PartyController.cs

#### ⚠️ 改善点

**1. HTTPステータスコードの不適切な使用 (行29)**

```csharp
if (string.IsNullOrEmpty(playerId))
{
    return Unauthorized(); // 401
}
```

**問題**: 認証済みだがplayerIdが取得できない場合は`Unauthorized`ではなく`BadRequest`または`InternalServerError`が適切

**提案**:

```csharp
if (string.IsNullOrEmpty(playerId))
{
    _logger.LogError("Authenticated user has no playerId");
    return StatusCode(500, "ユーザー情報の取得に失敗しました");
}
```

**2. 重複クエリ (行54, 61)**

```csharp
var partyCount = await _pokemonRepository.GetPartyCountAsync(playerId);
// ...
var party = await _pokemonRepository.GetPlayerPartyAsync(playerId);
```

**提案**: 1回のクエリで取得

```csharp
var party = await _pokemonRepository.GetPlayerPartyAsync(playerId);
if (party.Count <= 1)
{
    return BadRequest("最後のポケモンは逃がせません");
}
```

### 4.2 StarterController.cs


#### ⚠️ 改善点

**1. マジックナンバー (行18)**

```csharp
private static readonly int[] StarterSpeciesIds = { 390, 7, 495 };
```

**提案**: コメントを追記しておく.

**2. レベルのハードコーディング (行86)**

```csharp
Level = 5,
```

**提案**: 定数化する.

```csharp
private const int StarterPokemonLevel = 5;
```

### 4.3 GachaController.cs

#### ⚠️ 改善点

**1. マジックナンバー (行16, 69)**

```csharp
private const int GachaCost = 5000;
var randomLevel = random.Next(1, 11); // レベル1-10
```

**提案**: 設定ファイル化

```csharp
private readonly int _gachaCost;
private readonly int _minGachaLevel;
private readonly int _maxGachaLevel;

public GachaController(IOptions<GachaSettings> gachaSettings, ...)
{
    _gachaCost = gachaSettings.Value.Cost;
    _minGachaLevel = gachaSettings.Value.MinLevel;
    _maxGachaLevel = gachaSettings.Value.MaxLevel;
}
```

**2. トランザクション不足**

所持金減算とポケモン追加が別々のトランザクション

**提案**: トランザクションスコープを使用

```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    player.Money -= GachaCost;
    await _playerRepository.UpdateAsync(player);
    await _pokemonRepository.AddToPartyAsync(playerId, pokemon);
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

**3. Randomインスタンスの問題**

メソッド内で`new Random()`を生成している (行67)

**提案**: DIでシングルトンとして注入

### 4.4 BattleController.cs

#### ⚠️ 改善点

**1. PlayerId取得方法の不一致**

他のコントローラーは`User.Identity?.Name`を使用しているが、ここでは`User.FindFirstValue(ClaimTypes.NameIdentifier)`を使用

**提案**: 統一する

```csharp
var playerId = User.Identity?.Name;
```

**2. エラーハンドリング不足**

`CreateCpuBattleAsync`が例外をスローする可能性があるが、try-catchがない

**提案**:

```csharp
try
{
    var battleState = await _battleService.CreateCpuBattleAsync(playerId);
    return Created($"/api/battle/{battleState.BattleId}", battleState);
}
catch (InvalidOperationException ex)
{
    return BadRequest(ex.Message);
}
```

---

## 総合評価

### 🎯 全体的な強み

1. **Clean Architecture**: レイヤー分離が適切
2. **依存性注入**: インターフェースを活用した疎結合な設計
3. **ビジネスロジックの実装**: 戦闘システムが詳細に実装されている

### ⚠️ 主要な改善項目

#### 優先度: 高

1. **マジックナンバーの排除**: 定数化または設定ファイル化
2. **CPUBattlePlayerのバグ修正**: `Value`に正しい値を設定
3. **デバッグログの削除**: 本番環境用のログに置き換え
4. **トランザクション管理**: ガチャなど金銭が絡む処理
5. **エラーハンドリングの強化**: 適切な例外処理とログ出力

#### 優先度: 中

6. **重複コードの削減**: メソッド抽出によるDRY原則の適用
7. **N+1問題の解消**: EF CoreのIncludeを適切に使用
8. **HTTPステータスコードの適正化**: 状況に応じた適切なコード
9. **Randomインスタンスの管理**: シングルトンまたはクラスフィールド化

#### 優先度: 低

10. **タイプ相性テーブルの外部化**: JSONファイル化
11. **CPU AIの改善**: より戦略的な行動選択
12. **設定の外部化**: appsettings.jsonへの移行

### 📊 コード品質メトリクス

- **可読性**: ⭐⭐⭐⭐☆ (4/5)
- **保守性**: ⭐⭐⭐☆☆ (3/5) - マジックナンバーが多い
- **拡張性**: ⭐⭐⭐⭐☆ (4/5) - インターフェース活用
- **パフォーマンス**: ⭐⭐⭐☆☆ (3/5) - N+1問題あり
- **エラーハンドリング**: ⭐⭐⭐☆☆ (3/5) - 不足箇所あり

---

## 推奨アクションプラン

### フェーズ1: バグ修正 (即時対応)

- [ ] `CPUBattlePlayer.cs`のValue設定バグ修正
- [ ] デバッグログの削除または適切なロガーへの置き換え

### フェーズ2: コード品質向上 (1週間以内)

- [ ] マジックナンバーの定数化
- [ ] 重複コードのリファクタリング
- [ ] エラーハンドリングの追加

### フェーズ3: パフォーマンス改善 (2週間以内)

- [ ] N+1問題の解消
- [ ] トランザクション管理の強化
- [ ] 不要なクエリの削減

### フェーズ4: アーキテクチャ改善 (1ヶ月以内)

- [ ] 設定の外部化
- [ ] 計算ロジックの専用サービス化
- [ ] CPU AIの改善

---

**レビュー担当**: Claude (Antigravity AI)  
**レビュー完了日**: 2025-12-02
