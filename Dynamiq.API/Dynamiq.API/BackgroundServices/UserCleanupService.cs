using Dynamiq.API.Commands.User;
using Dynamiq.API.Interfaces;
using MediatR;

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
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var countOfRemoved = await mediator.Send(new RemoveAllExpiredUsersCommand());

                    _logger.LogInformation($"Count of expiredUsers removed: {countOfRemoved}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

}
