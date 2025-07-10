using Dynamiq.API.Interfaces;

namespace Dynamiq.API.BackgroundServices
{
    public class UserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;

        public UserCleanupService(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
                    await userRepo.RemoveAllExpiredUsers();
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
