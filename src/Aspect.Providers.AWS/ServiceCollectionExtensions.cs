using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Resources.EC2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspect.Providers.AWS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAWSCloudProvider(this IServiceCollection services)
        {
            services.TryAddSingleton<IAccountIdentityProvider<AwsAccount, AwsAccountIdentifier>, AWSAccountIdentityProvider>();

            return services.AddResourceExplorer<SecurityGroupResourceExplorer>()
                .AddCloudProvider<AWSCloudProvider>();
        }

        private static IServiceCollection AddResourceExplorer<T>(this IServiceCollection services)
            where T : class, IResourceExplorer<AwsAccount, AwsAccountIdentifier>
        {
            services.TryAddSingleton<IResourceExplorer<AwsAccount, AwsAccountIdentifier>, T>();
            return services;
        }
    }
}
