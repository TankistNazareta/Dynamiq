using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Security.Cryptography;

namespace Dynamiq.API.Tests;

public sealed class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private readonly string _dbName = $"DynamiqTests_{Guid.NewGuid():N}";
    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        var masterConnectionString = IsGitHub()
            ? "Server=localhost,1433;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
            : "Server=DESKTOP-HPNA4RC;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

        await using var connection = new SqlConnection(masterConnectionString);
        await connection.OpenAsync();

        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE [{_dbName}]";
            await cmd.ExecuteNonQueryAsync();
        }

        var csBuilder = new SqlConnectionStringBuilder(masterConnectionString)
        {
            InitialCatalog = _dbName,
            MultipleActiveResultSets = true
        };

        _connectionString = csBuilder.ToString();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["JwtSettings:Issuer"] = "https://api.dynamiq-test.fun",
                ["JwtSettings:Audience"] = "https://dynamiq-test.fun",
                ["JwtSettings:Key"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                ["AllowedHosts"] = "*"
            });
        });

        builder.ConfigureServices(services =>
        {
            RemoveService<DbContextOptions<AppDbContext>>(services);

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(_connectionString));

            ReplaceWithMock<IEmailService>(services);

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            services.PostConfigure<AuthenticationOptions>(o =>
            {
                o.DefaultAuthenticateScheme = "Test";
                o.DefaultChallengeScheme = "Test";
            });

            RemoveHostedServices(services);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        DropDatabase();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    private void DropDatabase()
    {
        var masterConnectionString = IsGitHub()
            ? "Server=localhost,1433;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
            : "Server=DESKTOP-HPNA4RC;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

        for (var i = 0; i < 5; i++)
        {
            try
            {
                using var connection = new SqlConnection(masterConnectionString);
                connection.Open();

                using var cmd = connection.CreateCommand();
                cmd.CommandText = $@"
                IF DB_ID('{_dbName}') IS NOT NULL
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END";
                cmd.ExecuteNonQuery();

                return;
            }
            catch
            {
                Thread.Sleep(500);
            }
        }
    }

    private static bool IsGitHub() =>
        Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

    private static void RemoveService<T>(IServiceCollection services)
    {
        var d = services.SingleOrDefault(s => s.ServiceType == typeof(T));
        if (d != null) services.Remove(d);
    }

    private static void ReplaceWithMock<T>(IServiceCollection services)
        where T : class
    {
        var d = services.SingleOrDefault(s => s.ServiceType == typeof(T));
        if (d != null) services.Remove(d);

        var mock = new Mock<T>();
        services.AddSingleton(mock.Object);
    }

    private static void RemoveHostedServices(IServiceCollection services)
    {
        var hosted = services
            .Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType))
            .ToList();

        foreach (var h in hosted)
            services.Remove(h);
    }
}
