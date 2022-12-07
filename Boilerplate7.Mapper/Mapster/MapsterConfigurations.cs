using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Boilerplate7.Mapper.Mapster
{
    public static class MapsterConfigurations
    {
        public static void InitializeMappingConfiguration(IServiceCollection services)
        {
            // Standaard configuratie
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

            var logger = services.BuildServiceProvider()
                .GetService<ILogger<MapsterAssemblyScanner>>();

            if (logger == null)
            {
                throw new InvalidOperationException("Logger needs to be loaded before the mapper is loaded");
            }

            var MapsterAssemblyScanner = new MapsterAssemblyScanner(logger);

            // Scan en registreer alle DLL's. 
            MapsterAssemblyScanner.ScanAndRegister();
        }


        public static IServiceCollection AddMapsterConfigurations(this IServiceCollection services)
        {
            InitializeMappingConfiguration(services);

            return services;
        }
    }
}
