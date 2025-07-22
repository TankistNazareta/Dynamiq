using Dynamiq.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dynamiq.Infrastructure.Extensions
{
    public static class PersistenceServiceCollectionExtension
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(connectionString, ss =>
                {
                    ss.EnableRetryOnFailure(3);
                }).EnableSensitiveDataLogging());
        }
    }
}
