using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.DAL
{
    public static class DI
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<AppDbContext>(option =>
                option.UseSqlServer(connectionString, ss =>
                {
                    ss.EnableRetryOnFailure(3);
                }).EnableSensitiveDataLogging());
        }
    }
}
