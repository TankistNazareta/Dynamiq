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

namespace Dynamiq.API.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime
        where TProgram : class
    {
        private string? _connectionString;
        private const string TestDbName = "DynamiqTestDb";
        private static bool _dbInitialized;
        private static readonly object _lock = new();

        public CustomWebApplicationFactory()
        {
            var masterConnectionString = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true"
                ? "Server=localhost,1433;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
                : "Server=DESKTOP-HPNA4RC;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

            var builder = new SqlConnectionStringBuilder(masterConnectionString)
            {
                InitialCatalog = TestDbName,
                MultipleActiveResultSets = true
            };

            _connectionString = builder.ToString();
        }

        public async Task InitializeAsync()
        {
            if (_dbInitialized) return;

            lock (_lock)
            {
                if (_dbInitialized) return;

                using var connection = new SqlConnection(_connectionString.Replace($"Database={TestDbName}", "Database=master"));
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $@"
                    IF DB_ID(N'{TestDbName}') IS NULL
                        CREATE DATABASE [{TestDbName}];
                    ALTER AUTHORIZATION ON DATABASE::[{TestDbName}] TO [sa];";
                    command.ExecuteNonQuery();
                }

                _dbInitialized = true;
            }

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
                    db.Database.Migrate();

                    break;
                }
                catch
                {
                    retries--;
                    if (retries == 0) throw;
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

                // Mock Email service
                var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
                if (emailDescriptor != null) services.Remove(emailDescriptor);

                var mockEmail = new Mock<IEmailService>();
                mockEmail.Setup(m => m.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
                services.AddSingleton(mockEmail.Object);

                // Remove hosted services (щоб не стартували background job-и)
                var hostedServices = services.Where(s => typeof(IHostedService).IsAssignableFrom(s.ServiceType)).ToList();
                foreach (var hs in hostedServices)
                    services.Remove(hs);
            });
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
