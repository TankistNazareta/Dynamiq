using Dynamiq.Auth.DAL.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.Auth.DAL
{
    public static class DI
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<AuthDbContext>(option =>
                option.UseSqlServer(connectionString, ss =>
                {
                    ss.EnableRetryOnFailure(3);
                }).EnableSensitiveDataLogging());
        }
    }
}
