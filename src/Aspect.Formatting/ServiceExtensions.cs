using Aspect.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspect.Formatting
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddFormatters(this IServiceCollection services)
        {
            services.TryAddSingleton<IFormatterFactory, FormatterFactory>();
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IFormatter, JsonFormatter>());
            services.TryAddEnumerable(ServiceDescriptor.Scoped<IFormatter, YamlFormatter>());
            return services;
        }
    }
}
