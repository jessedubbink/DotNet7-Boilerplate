using Boilerplate7.Entity;
using Boilerplate7.Entity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate7.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(o => o.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // services.AddTransient<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            // services.AddTransient<IValidator<User>, UserValidator>();
            return services;
        }
    }
}
