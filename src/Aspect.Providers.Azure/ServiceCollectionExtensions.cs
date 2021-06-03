using Aspect.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Aspect.Providers.Azure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureCloudProvider(this IServiceCollection services)
        {
            return services.AddCloudProvider<AzureCloudProvider>();
        }
    }
}
