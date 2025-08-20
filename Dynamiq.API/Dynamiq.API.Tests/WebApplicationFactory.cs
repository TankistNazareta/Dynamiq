using Dynamiq.Application.Interfaces.Services;
using Dynamiq.Domain.Interfaces;
using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dynamiq.API.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime
        where TProgram : class
    {
        private string? _connectionString;
        private const string TestDbName = "DynamiqTestDb";

        public string ConnectionString => _connectionString ??
            throw new InvalidOperationException("Connection string is not initialized.");

        public async Task EnsureDatabaseReadyAsync()
        {
            var retries = 5;
            while (retries > 0)
            {
                try
                {
                    var options = new DbContextOptionsBuilder<AppDbContext>()
                        .UseSqlServer(_connectionString)
                        .Options;

                    using var scope = Services.CreateScope();
                    var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
                    using var db = new AppDbContext(options, dispatcher);
                    await db.Database.EnsureCreatedAsync();
                    return;
                }
                catch
                {
                    retries--;
                    await Task.Delay(5000);
                }
            }

            throw new Exception("Cannot connect to SQL Server.");
        }

        public async Task InitializeAsync()
        {
            var masterConnectionString = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
                ? "Server=localhost,1433;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
                : "Server=DESKTOP-HPNA4RC;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

            await using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            await using (var command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    IF DB_ID(N'{TestDbName}') IS NULL
                        CREATE DATABASE [{TestDbName}];";
                await command.ExecuteNonQueryAsync();
            }

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = TestDbName,
                MultipleActiveResultSets = true
            };
            _connectionString = builder.ToString();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var scope = Services.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IDomainEventDispatcher>();
            using var db = new AppDbContext(options, dispatcher);
            await db.Database.MigrateAsync();
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

                var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null) services.Remove(emailDescriptor);

                var mockEmail = new Mock<IEmailService>();
                mockEmail.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
                services.AddSingleton(mockEmail.Object);

                var hostedServices = services.Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType)).ToList();
                foreach (var hs in hostedServices)
                    services.Remove(hs);
            });
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
}
