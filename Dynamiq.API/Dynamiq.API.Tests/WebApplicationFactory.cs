using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Threading;
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

        private string? _connectionString;
        private string? _dbName;

        public string ConnectionString => _connectionString ?? throw new InvalidOperationException("Connection string is not initialized yet.");

        public CustomWebApplicationFactory()
        {
            // Заблокуємо створення хоста, поки не ініціалізуємо контейнер і БД
            InitializeAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            // Запускаємо контейнер (StartAsync ігнорує, якщо він вже запущений)
            await _dbContainer.StartAsync();

            // Створюємо унікальну тестову базу
            var masterConnectionString = _dbContainer.GetConnectionString();
            await using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            _dbName = $"TestDb_{Guid.NewGuid():N}";

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE [{_dbName}];";
                await command.ExecuteNonQueryAsync();
            }

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = _dbName,
                MultipleActiveResultSets = true
            };
            _connectionString = builder.ToString();

            // Переконуємося, що база доступна
            var retries = 5;
            while (retries-- > 0)
            {
                try
                {
                    await using var testConn = new SqlConnection(_connectionString);
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
                // Видаляємо реєстрацію DbContext, якщо є
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                // Додаємо DbContext з тестовим connection string
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString!));

                // Створюємо схему БД після побудови сервісів
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();

                // Мокаємо IEmailService
                var emailDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null)
                    services.Remove(emailDescriptor);

                var mockEmail = new Mock<IEmailService>();
                mockEmail.Setup(p => p.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
                services.AddSingleton(mockEmail.Object);

                // Видаляємо всі HostedService (щоб не чіплялись до реальної БД)
                var hostedServices = services
                    .Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType))
                    .ToList();

                foreach (var hostedService in hostedServices)
                    services.Remove(hostedService);
            });
        }

        public async Task DisposeAsync()
        {
            if (!string.IsNullOrEmpty(_dbName))
            {
                var masterConnectionString = _dbContainer.GetConnectionString();
                await using var connection = new SqlConnection(masterConnectionString);
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = $@"
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];";
                await command.ExecuteNonQueryAsync();
            }

            await _dbContainer.StopAsync();
        }
    }
}
