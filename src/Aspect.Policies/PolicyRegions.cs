using System.Collections.Generic;
using System.Linq;

namespace Aspect.Policies
{
    internal static class PolicyRegions
    {
        internal static IReadOnlyList<string> All => AWS.All.Concat(Azure.All).ToList();
        internal static AwsPolicyRegions AWS { get; } = new AwsPolicyRegions();
        internal static AzurePolicyRegions Azure { get; } = new AzurePolicyRegions();

        internal class AwsPolicyRegions
        {
            // https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/using-regions-availability-zones.html#concepts-available-regions
            // Only non-opt in regions listed
            public IEnumerable<string> NorthAmerica { get; } = new[] { "us-east-1", "us-east-2", "us-west-1", "us-west-2", "ca-central-1" };
            public IEnumerable<string> SouthAmerica { get; } = new[] { "sa-east-1" };
            public IEnumerable<string> EMEA { get; } = new[] { "eu-central-1", "eu-west-1", "eu-west-2", "eu-west-3", "eu-north-1" };
            public IEnumerable<string> Asia { get; } = new[] { "ap-south-1", "ap-northeast-3", "ap-northeast-2", "ap-northeast-1", "ap-southeast-1", "ap-southeast-2" };
            public IEnumerable<string> All => EMEA.Concat(Asia).Concat(SouthAmerica).Concat(NorthAmerica);
        }

        internal class AzurePolicyRegions
        {
            public IEnumerable<string> All => Enumerable.Empty<string>();
        }
    }
}
