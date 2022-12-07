using Boilerplate7.Mapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate7.Configuration
{
    public static class DefaultConfiguration
    {
        public static IServiceCollection Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDatabaseContext(configuration.GetConnectionString("Default"));
            services.AddMappingConfigurations();

            return services;
        }
    }
}
