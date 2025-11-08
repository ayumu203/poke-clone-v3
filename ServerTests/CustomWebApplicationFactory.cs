using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using server.Data;
using server.Helpers;

namespace ServerTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
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
            // 既存の設定ソースをクリアして、テスト用設定のみを使用
            config.Sources.Clear();
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["IsAuthenticationEnabled"] = "false",  // 認証を完全に無効化
                ["Jwt:Key"] = "test-secret-key-for-jwt-authentication-minimum-32-characters",
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience"
            });
        });

        builder.ConfigureServices((context, services) =>
        {
            // DbContextを削除
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // インメモリDBを追加（コンストラクタで指定されたDB名を使用）
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });
        });

        base.ConfigureWebHost(builder);
    }
}
