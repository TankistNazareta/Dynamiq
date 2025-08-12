using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Application.Interfaces.Stripe;
using Dynamiq.Infrastructure.Persistence.Context;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.MsSql;

namespace Dynamiq.API.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private static readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrongPassword123!")
            .Build();

        private string _connectionString;
        private string _dbName;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            var masterConnectionString = _dbContainer.GetConnectionString();

            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            _dbName = $"TestDb_{Guid.NewGuid():N}";

            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE DATABASE [{_dbName}];";
            await command.ExecuteNonQueryAsync();

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = _dbName,
                MultipleActiveResultSets = true
            };
            _connectionString = builder.ToString();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString));

                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                db.Database.EnsureCreated();
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (descriptor != null)
                    services.Remove(descriptor);

                var mockParser = new Mock<IEmailService>();
                mockParser.Setup(p => p.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

                services.AddSingleton(mockParser.Object);
            });
        }

        public async Task DisposeAsync()
        {
            if (!string.IsNullOrEmpty(_dbName))
            {
                var masterConnectionString = _dbContainer.GetConnectionString();
                using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = $@"
                ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{_dbName}];";
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}