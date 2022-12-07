using Mapster;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace Boilerplate7.Mapper.Mapster
{
    public class MapsterAssemblyScanner
    {
        private readonly ILogger<MapsterAssemblyScanner> _logger;
        private const string AssemblyNameFilter = "Boilerplate7.";
        private static readonly ConcurrentBag<TypeInfo> RegisteredTypes = new();

        public MapsterAssemblyScanner(ILogger<MapsterAssemblyScanner> logger)
        {
            _logger = logger;
        }

        private IEnumerable<TypeInfo> GetAssemblyTypes(Assembly assembly)
        {
            var interfaceType = typeof(IRegister).GetTypeInfo();

            IEnumerable<TypeInfo> types;

            try
            {
                types = assembly.GetTypes().Where(i => i.IsAbstract is false && interfaceType.IsAssignableFrom(i)).Select(i => i.GetTypeInfo());
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                // Log alle excepties die we krijgen
                foreach (var loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    _logger.LogError(exception: loaderException, message: "Get assembly types encountered an error");
                }

                // Ookal zijn er meerdere dingen die niet kunnen inladen wegens verschillende redenen, we willen de rest wel inladen
                types = reflectionTypeLoadException.Types.Where(type => type != null &&
                                                                type.IsAbstract is false &&
                                                                interfaceType.IsAssignableFrom(type))
                                                         .Select(type => type!.GetTypeInfo());
            }

            return types;
        }

        private void RegisterTypesInfos(IEnumerable<TypeInfo> types)
        {
            foreach (var type in types)
            {
                try
                {
                    // Controleeer of het type niet al geregistreerd is. 2x doen is zonde
                    if (RegisteredTypes.Contains(type))
                    {
                        continue;
                    }

                    // Voeg toe aan geregistreerde lijst
                    RegisteredTypes.Add(type);

                    // Log dat er een nieuwe mapping gevonden is
                    _logger.LogDebug($"Apply mapping for {type.Name}");

                    // Registreer type
                    if (Activator.CreateInstance(type) is not IRegister instance)
                    {
                        throw new InvalidOperationException($"Can only implement {nameof(IRegister)} interfaces");
                    }

                    instance.Register(config: TypeAdapterConfig.GlobalSettings);

                    // Instance opruimen indien nodig.
                    if (instance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Error while registering types. {nameof(type)}: {type}");

                    //Zorg ervoor dat het opstarten mislukt door te throwen
                    throw;
                }
            }
        }

        public void ScanAndRegister()
        {
            var assemblyList = AppDomain.CurrentDomain.GetAssemblies()
                                                      .Where(assembly => assembly.FullName != null && assembly.FullName.StartsWith(AssemblyNameFilter, StringComparison.InvariantCulture))
                                                      .ToList();

            var types = new HashSet<TypeInfo>();
            var finishedAssemblyNames = new List<string>();

            foreach (var assembly in assemblyList)
            {
                try
                {
                    if (assembly.FullName != null && finishedAssemblyNames.Contains(assembly.FullName))
                    {
                        continue;
                    }

                    // Voeg types toe die gescanned zijn
                    foreach (var assemblyType in GetAssemblyTypes(assembly))
                    {
                        types.Add(item: assemblyType);
                    }

                    // Haal assemblies op
                    var referencedAssemblies = assembly.GetReferencedAssemblies().Where(assemblyName => assemblyName.FullName.StartsWith(value: AssemblyNameFilter, comparisonType: StringComparison.InvariantCulture));
                    foreach (var referencedAssembly in referencedAssemblies)
                    {
                        try
                        {
                            if (finishedAssemblyNames.Contains(referencedAssembly.FullName))
                            {
                                continue;
                            }

                            // Laad assembly
                            var referencedAssemblyLoaded = Assembly.Load(assemblyRef: referencedAssembly);

                            // Voeg type toe van gescande assembly
                            foreach (var assemblyType in GetAssemblyTypes(assembly: referencedAssemblyLoaded))
                            {
                                types.Add(item: assemblyType);
                            }
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError(exception, $"Got an error while loading assembly. {nameof(referencedAssembly)}, {referencedAssembly}");
                        }
                        finally
                        {
                            if (assembly.FullName != null)
                            {
                                finishedAssemblyNames.Add(item: assembly.FullName);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Unknown exception while searching through the assemblies");
                }
                finally
                {
                    if (assembly.FullName != null)
                    {
                        finishedAssemblyNames.Add(item: assembly.FullName);
                    }
                }
            }

            // Voeg alle initiele types toe
            RegisterTypesInfos(types);

            // Voeg dynamisch gelanden typen toe
            AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
            {
                try
                {
                    if (args.LoadedAssembly.FullName != null && args.LoadedAssembly.FullName.StartsWith(value: AssemblyNameFilter, comparisonType: StringComparison.InvariantCulture))
                    {
                        RegisterTypesInfos(types: GetAssemblyTypes(assembly: args.LoadedAssembly));

                        _logger.LogInformation($"All types loaded from assembly: {args.LoadedAssembly.FullName}");
                    }
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Exception while loading mapping, {nameof(args.LoadedAssembly)}: {args.LoadedAssembly}");
                }
            };

            foreach (var typeInfo in types)
            {
                _logger.LogInformation(message: $"Type {typeInfo.Name} loaded from assembly {typeInfo.Assembly.FullName}");
            }
        }
    }
}
