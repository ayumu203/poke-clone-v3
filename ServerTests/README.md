# APIテスト

## テスト構成

### 実装済みテスト

#### 統合テスト (`ApiIntegrationTests.cs`)

1. **Test1_CreatePlayer_ShouldReturnCreatedPlayer**
   - プレイヤー作成APIのテスト
   - POST `/api/players`
   - 期待: 201 Created

2. **Test2_GetPlayer_ShouldReturnPlayerInfo**
   - プレイヤー情報取得APIのテスト
   - GET `/api/players/{id}`
   - 期待: 200 OK、JSONデータの比較

3. **Test3_AddPokemonToParty_ShouldReturnCreatedPokemon**
   - パーティへのポケモン追加APIのテスト
   - POST `/api/players/{id}/party`
   - 期待: 201 Created

4. **Test4_GetParty_ShouldReturnPartyList**
   - パーティ一覧取得APIのテスト
   - GET `/api/players/{id}/party`
   - 期待: 200 OK、1匹のポケモンを含む

#### スナップショットテスト (`ApiSnapshotTests.cs`)

1. **CreatePlayer_ShouldMatchSnapshot**
   - プレイヤー作成レスポンスのスナップショット検証

2. **GetPlayer_ShouldMatchSnapshot**
   - プレイヤー情報取得レスポンスのスナップショット検証

3. **GetParty_ShouldMatchSnapshot**
   - パーティ一覧取得レスポンスのスナップショット検証

### 使用技術・ライブラリ

- **xUnit** (2.9.3) - テストフレームワーク
- **Microsoft.AspNetCore.Mvc.Testing** (8.0.11) - 統合テスト用WebApplicationFactory
- **Microsoft.EntityFrameworkCore.InMemory** (9.0.10) - インメモリデータベース
- **FluentAssertions** (8.8.0) - 流暢なアサーション
- **Verify.Xunit** (31.4.3) - スナップショットテスト
- **System.IdentityModel.Tokens.Jwt** - JWT認証トークン生成

## テスト実行方法

```bash
# テストプロジェクトに移動
cd ServerTests

# すべてのテストを実行
dotnet test

# 詳細な出力で実行
dotnet test --logger "console;verbosity=detailed"

# 特定のテストのみ実行
dotnet test --filter "Test1_CreatePlayer"

# カバレッジ付きで実行
dotnet test --collect:"XPlat Code Coverage"
```

## 新しいテストの作成方法

### 1. 統合テストの作成

#### 基本的な構造

```csharp
[Fact]
public async Task TestName_ShouldExpectBehavior()
{
    // Arrange - テストの準備
    SetupAuthentication();  // 認証トークンを設定
    
    // 必要に応じてプレイヤーやデータを作成
    var createPlayerDto = await LoadTestDataAsync<CreatePlayerDto>("player.json");
    await _client.PostAsJsonAsync("/api/players", createPlayerDto);
    
    // Act - テスト対象の実行
    var response = await _client.GetAsync($"/api/players/{_testPlayerId}");
    
    // Assert - 結果の検証
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<PlayerDto>();
    result.Should().NotBeNull();
    result!.PlayerId.Should().Be(_testPlayerId);
}
```

#### ポイント

1. **認証の設定**: 各テストで`SetupAuthentication()`を呼び出す
2. **データの準備**: 各テストは独立したデータベースを持つため、必要なデータを毎回作成
3. **テストデータ**: `TestData/`フォルダのJSONファイルを活用
4. **アサーション**: FluentAssertionsで読みやすい検証

### 2. スナップショットテストの作成

```csharp
[Fact]
public async Task FeatureName_ShouldMatchSnapshot()
{
    // Arrange
    SetupAuthentication();
    // 必要なデータを準備...
    
    // Act
    var response = await _client.GetAsync("/api/endpoint");
    var result = await response.Content.ReadFromJsonAsync<ResponseDto>();
    
    // Assert - スナップショットと比較
    await Verify(result)
        .UseDirectory("Snapshots")
        .UseMethodName("FeatureName");
}
```

#### スナップショット承認の流れ

1. テスト実行時に`.received.txt`ファイルが生成される
2. 内容を確認して正しければ、`.verified.txt`にコピー

   ```bash
   cp Snapshots/TestName.received.txt Snapshots/TestName.verified.txt
   ```

3. 次回以降、`.verified.txt`と比較される

### 3. テストデータの追加

`TestData/`フォルダに新しいJSONファイルを追加：

```json
// TestData/new_data.json
{
  "field1": "value1",
  "field2": 123
}
```

`.csproj`ファイルで自動コピーを設定（既に設定済み）：

```xml
<ItemGroup>
  <None Update="TestData\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

テスト内で読み込み：

```csharp
var data = await LoadTestDataAsync<MyDto>("new_data.json");
```

## アーキテクチャ設計

### テスト分離戦略

各テストは完全に独立したデータベースを使用します：

```csharp
public class ApiIntegrationTests : IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    
    public ApiIntegrationTests()
    {
        // 各テストインスタンスごとにユニークなDB
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
```

**利点**:

- テスト間のデータ競合がない
- 並列実行が可能
- テストの順序に依存しない

### CustomWebApplicationFactory

テスト環境に特化した設定を提供：

```csharp
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
{
    private readonly string _databaseName;
    
    public CustomWebApplicationFactory(string? databaseName = null)
    {
        _databaseName = databaseName ?? $"TestDatabase_{Guid.NewGuid()}";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["IsAuthenticationEnabled"] = "false",
                ["Jwt:Key"] = "test-secret-key...",
                // ...
            });
        });
        
        builder.ConfigureServices((context, services) =>
        {
            // SQL Serverを削除してInMemory DBに置き換え
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });
    }
}
```

### 認証とJWT

テスト環境では`NoAuthHandler`を使用して認証をバイパス：

- **Program.cs**: Test環境では`IsAuthenticationEnabled = false`
- **NoAuthHandler**: "test-player-id"を持つClaimsPrincipalを返す
- **テスト内**: 実際のJWTトークンを生成（将来の拡張性のため）

## トラブルシューティング

### 1. 認証エラー（401 Unauthorized）

**原因**: `SetupAuthentication()`が呼ばれていない

**解決策**: 各テストの最初で認証を設定

```csharp
SetupAuthentication();
```

### 2. データベースプロバイダーの競合

**原因**: SQL ServerとInMemory DBが両方登録されている

**解決**: `Program.cs`でTest環境の場合はSQL Serverをスキップ

```csharp
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(...);
}
```

## ベストプラクティス

### 1. テスト命名規則

```bash
[テストメソッド]_[期待する動作]
例: Test1_CreatePlayer_ShouldReturnCreatedPlayer
```

### 2. Arrange-Act-Assert パターン

```csharp
// Arrange - テストの準備
SetupAuthentication();
var data = await LoadTestDataAsync<Dto>("data.json");

// Act - テスト対象の実行
var response = await _client.PostAsJsonAsync("/api/endpoint", data);

// Assert - 結果の検証
response.StatusCode.Should().Be(HttpStatusCode.Created);
```

### 3. テストデータの管理

- JSONファイルで管理（バージョン管理しやすい）
- 再利用可能なデータを`TestData/`に配置
- テスト固有のデータはテスト内で直接定義

### 4. スナップショットの更新

- 意図的な変更の場合のみ`.verified.txt`を更新
- 大きな変更の場合はレビュー時に`.received.txt`と`.verified.txt`の差分を確認

### 5. テストの独立性

- 各テストは他のテストに依存しない
- 必要なデータは毎回作成
- 共有状態を避ける

## ディレクトリ構造

```bash
ServerTests/
├── ApiIntegrationTests.cs      # 統合テスト
├── ApiSnapshotTests.cs         # スナップショットテスト
├── CustomWebApplicationFactory.cs  # テスト用ファクトリ
├── TestData/                   # テストデータ
│   ├── player.json
│   ├── expected_player.json
│   ├── pokemon.json
│   └── pokemon_species.json
├── Snapshots/                  # スナップショットファイル
│   ├── ApiSnapshotTests.CreatePlayer.verified.txt
│   ├── ApiSnapshotTests.GetPlayer.verified.txt
│   └── ApiSnapshotTests.GetParty.verified.txt
└── ServerTests.csproj          # プロジェクトファイル
```

## 参考リンク

### テストフレームワーク・ライブラリ

- [xUnit Documentation](https://xunit.net/) - .NETのテストフレームワーク
  - [Getting Started](https://xunit.net/docs/getting-started/netcore/cmdline)
  - [Shared Context between Tests](https://xunit.net/docs/shared-context)
  
- [FluentAssertions Documentation](https://fluentassertions.com/) - 読みやすいアサーションライブラリ
  - [Collections](https://fluentassertions.com/collections/)
  - [Exception Assertions](https://fluentassertions.com/exceptions/)
  
- [Verify Documentation](https://github.com/VerifyTests/Verify) - スナップショットテスト
  - [Getting Started with Xunit](https://github.com/VerifyTests/Verify/blob/main/docs/xunit.md)
  - [Naming Conventions](https://github.com/VerifyTests/Verify/blob/main/docs/naming.md)

### ASP.NET Core テスト

- [ASP.NET Core Integration Tests](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests) - Microsoft公式ドキュメント
- [WebApplicationFactory](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests#basic-tests-with-the-default-webapplicationfactory) - テスト用ファクトリ
- [Test middleware](https://learn.microsoft.com/en-us/aspnet/core/test/middleware) - ミドルウェアのテスト

### Entity Framework Core テスト

- [Testing EF Core Applications](https://learn.microsoft.com/en-us/ef/core/testing/) - EFCoreテスト概要
- [Testing with InMemory](https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#inmemory-provider) - InMemoryプロバイダー
- [Testing with SQLite](https://learn.microsoft.com/en-us/ef/core/testing/testing-without-the-database#sqlite-in-memory) - SQLiteを使った代替手法

### テストのベストプラクティス・参考資料

- [Unit testing best practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices) - .NET単体テストのベストプラクティス
- [Test-Driven Development (TDD)](https://learn.microsoft.com/en-us/visualstudio/test/quick-start-test-driven-development-with-test-explorer) - TDD入門

### CI/CD

- [GitHub Actions for .NET](https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net) - GitHub Actionsでの.NETビルド・テスト
- [Azure Pipelines for .NET](https://learn.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core) - Azure Pipelinesでの.NETビルド
