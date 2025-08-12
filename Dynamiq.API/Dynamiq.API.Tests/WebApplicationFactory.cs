using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

        private string _connectionString = null!;
        private string _dbName = null!;

        public string ConnectionString => _connectionString;

        public async Task InitializeAsync()
        {
            // Стартуємо MSSQL контейнер
            await _dbContainer.StartAsync();

            // Створюємо тестову БД
            var masterConnectionString = _dbContainer.GetConnectionString();
            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            _dbName = $"TestDb_{Guid.NewGuid():N}";

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE [{_dbName}];";
                await command.ExecuteNonQueryAsync();
            }

            // Формуємо connection string для нової БД
            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = _dbName,
                MultipleActiveResultSets = true
            };
            _connectionString = builder.ToString();

            // Health-check — чекаємо, поки БД стане доступною
            var retries = 5;
            while (retries-- > 0)
            {
                try
                {
                    using var testConn = new SqlConnection(_connectionString);
                    await testConn.OpenAsync();
                    break;
                }
                catch
                {
                    await Task.Delay(2000);
                }
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // Видаляємо стандартний DbContext
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                // Реєструємо AppDbContext з тестовою БД (вже готовий connection string)
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString));

                // Створюємо схему БД
                using (var scope = services.BuildServiceProvider().CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.Migrate();
                }

                // Мокаємо EmailService
                var emailDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null)
                    services.Remove(emailDescriptor);

                var mockEmail = new Mock<IEmailService>();
                mockEmail.Setup(p => p.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
                services.AddSingleton(mockEmail.Object);

                // Прибираємо всі HostedService (щоб не стукались у реальну БД)
                var hostedServices = services
                    .Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType))
                    .ToList();

                foreach (var hostedService in hostedServices)
                {
                    services.Remove(hostedService);
                }
            });
        }

        public async Task DisposeAsync()
        {
            // Видаляємо тестову БД після завершення
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

            await _dbContainer.StopAsync();
        }
    }
}
