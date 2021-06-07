using System.Collections.Generic;

namespace Aspect.Providers.AWS.Models.EC2
{
    public class AwsVpcResource : AwsResource
    {
        public AwsVpcResource(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsVpcResource), region)
        {
        }
    }
}