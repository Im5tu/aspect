using Microsoft.Extensions.DependencyInjection;

namespace Aspect.Abstractions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddResourceExplorer<T>(this IServiceCollection services)
            where T : class, IResourceExplorer
            => services.AddSingleton<IResourceExplorer, T>();
    }
}
