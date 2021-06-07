using System.Collections.Generic;

namespace Aspect.Providers.AWS.Models.EC2
{
    public class AwsVpcEndpoint : AwsResource
    {
        public AwsVpcEndpoint(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsVpcEndpoint), region)
        {
        }
    }
}