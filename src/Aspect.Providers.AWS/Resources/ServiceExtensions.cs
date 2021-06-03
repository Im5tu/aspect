using Aspect.Abstractions;
using Aspect.Providers.AWS.Resources.EC2;
using Microsoft.Extensions.DependencyInjection;

namespace Aspect.Providers.AWS.Resources
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAWSResourceExplorers(this IServiceCollection services)
        {
            return services.AddResourceExplorer<SecurityGroupResourceExplorer>();
        }
    }
}
