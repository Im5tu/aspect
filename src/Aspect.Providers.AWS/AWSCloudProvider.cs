using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS
{
    internal sealed class AWSCloudProvider : ICloudProvider
    {
        private readonly IAccountIdentityProvider<AwsAccount, AwsAccount.AwsAccountIdentifier> _accountIdentityProvider;
        private readonly Dictionary<string,Type> _resources;
        private readonly Dictionary<Type, IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier>> _resourceProviders;

        // https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/using-regions-availability-zones.html#concepts-available-regions
        private readonly IEnumerable<string> NorthAmerica = new[] { "us-east-1", "us-east-2", "us-west-1", "us-west-2", "ca-central-1" };
        private readonly IEnumerable<string> SouthAmerica = new[] { "sa-east-1" };
        private readonly IEnumerable<string> EMEA = new[] { "eu-central-1", "eu-west-1", "eu-west-2", "eu-west-3", "eu-north-1" };
        private readonly IEnumerable<string> Asia = new[] { "ap-south-1", "ap-northeast-3", "ap-northeast-2", "ap-northeast-1", "ap-southeast-1", "ap-southeast-2" };
        private IEnumerable<string> All => EMEA.Concat(Asia).Concat(SouthAmerica).Concat(NorthAmerica);

        public Type AccountType { get; } = typeof(AwsAccount);
        public string Name { get; } = "AWS";

        public AWSCloudProvider(IEnumerable<IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier>> resourceProviders, IAccountIdentityProvider<AwsAccount, AwsAccount.AwsAccountIdentifier> accountIdentityProvider)
        {
            _resourceProviders = resourceProviders.ToDictionary(x => x.ResourceType, x => x);
            _accountIdentityProvider = accountIdentityProvider;
            _resources = typeof(AwsSecurityGroup).Assembly.GetResourceTypes().ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyDictionary<string, Type> GetResources() => _resources;
        public IEnumerable<string> GetAllRegions() => All.ToList();
        public IEnumerable<string> GetDefaultRegions()
        {
            var defaultRegion = Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");
            if (!string.IsNullOrWhiteSpace(defaultRegion) && All.Contains(defaultRegion, StringComparer.OrdinalIgnoreCase))
                return new[] {defaultRegion};

            return All.ToList();
        }

        public async Task<IEnumerable<IResource>> DiscoverResourcesAsync(string region, Type resourceType, Action<string> updater, CancellationToken cancellationToken)
        {
            updater("Getting account details from STS...");
            var account = await _accountIdentityProvider.GetAccountAsync(cancellationToken);

            updater($"Loading {resourceType.Name} resources in region {region}...");
            return await _resourceProviders[resourceType].DiscoverResourcesAsync(account, region, cancellationToken);
        }
    }
}
