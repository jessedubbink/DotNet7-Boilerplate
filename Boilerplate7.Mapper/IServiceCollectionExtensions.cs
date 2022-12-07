using Boilerplate7.Mapper.Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Boilerplate7.Mapper
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMappingConfigurations(this IServiceCollection services)
        {
            services.AddMapsterConfigurations();

            return services;
        }
    }
}