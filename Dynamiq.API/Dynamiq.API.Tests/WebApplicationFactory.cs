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
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private static readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("YourStrongPassword123!")
            .Build();

        private string? _connectionString;
        private string? _dbName;

        public string ConnectionString => _connectionString ?? throw new InvalidOperationException("Connection string is not initialized.");

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

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

            // Retry opening connection to new DB
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
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(_connectionString!));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();

                // Mock email service
                var emailServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (emailServiceDescriptor != null) services.Remove(emailServiceDescriptor);
                var mockEmail = new Mock<IEmailService>();
                mockEmail.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
                services.AddSingleton(mockEmail.Object);

                // Remove hosted services (if any)
                var hostedServices = services.Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType)).ToList();
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
