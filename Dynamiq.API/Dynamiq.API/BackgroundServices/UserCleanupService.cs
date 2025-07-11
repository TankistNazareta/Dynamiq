using Dynamiq.API.Interfaces;

namespace Dynamiq.API.BackgroundServices
{
    public class UserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public UserCleanupService(IServiceProvider services, ILogger<UserCleanupService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
                    var countOfRemoved = await userRepo.RemoveAllExpiredUsers();

                    _logger.LogInformation($"Count of expiredUsers removed: {countOfRemoved}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
