using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspect.Abstractions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection TryAddCoreServices(this IServiceCollection services)
        {
            services.TryAddSingleton<IResourceTypeLocator, ResourceTypeLocator>();
            services.TryAddSingleton<IReadOnlyDictionary<string, ICloudProvider>>(sp => sp.GetRequiredService<IEnumerable<ICloudProvider>>().ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase));
            return services;
        }

        public static IServiceCollection AddCloudProvider<T>(this IServiceCollection services)
            where T : class, ICloudProvider
            => services.AddSingleton<ICloudProvider, T>();
    }
}
