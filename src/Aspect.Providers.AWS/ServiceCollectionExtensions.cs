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
            services.TryAddSingleton<IAccountIdentityProvider<AwsAccount, AwsAccount.AwsAccountIdentifier>, AWSAccountIdentityProvider>();

            return services
                .AddResourceExplorer<AwsEc2ImageResourceExplorer>()
                .AddResourceExplorer<AwsEc2InstanceResourceExplorer>()
                .AddResourceExplorer<AwsEc2KeyPairResourceExplorer>()
                .AddResourceExplorer<AwsEc2SnapshotResourceExplorer>()
                .AddResourceExplorer<AwsEc2VolumeResourceExplorer>()
                .AddResourceExplorer<AwsElasticIpResourceExplorer>()
                .AddResourceExplorer<AwsInternetGatewayResourceExplorer>()
                .AddResourceExplorer<AwsNatGatewayResourceExplorer>()
                .AddResourceExplorer<AwsNetworkInterfaceResourceExplorer>()
                .AddResourceExplorer<AwsPrefixListResourceExplorer>()
                .AddResourceExplorer<AwsReservedInstanceResourceExplorer>()
                .AddResourceExplorer<AwsRouteTableResourceExplorer>()
                .AddResourceExplorer<AwsSecurityGroupResourceExplorer>()
                .AddResourceExplorer<AwsTransitGatewayResourceExplorer>()
                .AddResourceExplorer<AwsVpcEndpointResourceExplorer>()
                .AddResourceExplorer<AwsVpcPeeringConnectionResourceExplorer>()
                .AddResourceExplorer<AwsVpcResourceExplorer>()
                .AddCloudProvider<AWSCloudProvider>();
        }

        private static IServiceCollection AddResourceExplorer<T>(this IServiceCollection services)
            where T : class, IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier>
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier>, T>());
            return services;
        }
    }
}
