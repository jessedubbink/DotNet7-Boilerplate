using Mapster;
using System.Collections;

namespace Boilerplate7.Mapper.ExtensionMethods
{
    public static class MappingExtensions
    {
        public static TDestination? MapTo<TDestination>(this object source)
            where TDestination : class
        {
            return source.Adapt<TDestination>();
        }

        public static TDestination? MapTo<TDestination>(this IEnumerable<object> source)
            where TDestination : class, IEnumerable
        {
            return source.Adapt<TDestination>();
        }

        public static IQueryable<TDestination>? MapTo<TDestination>(this IQueryable source)
        {
            return source.ProjectToType<TDestination>();
        }
    }
}
