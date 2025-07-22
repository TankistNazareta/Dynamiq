using Dynamiq.Application.Interfaces.UseCases;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dynamiq.Infrastructure.BackgroundServices
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

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var userCleanupUseCase = scope.ServiceProvider.GetRequiredService<IUserCleanupUseCase>();

                    var countOfRemoved = await userCleanupUseCase.RemoveAllExpiredUsersAsync(cancellationToken);

                    _logger.LogInformation($"Count of expiredUsers removed: {countOfRemoved}");
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }
    }

}
