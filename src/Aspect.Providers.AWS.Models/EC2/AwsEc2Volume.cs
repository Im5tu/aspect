using System.Collections.Generic;

namespace Aspect.Providers.AWS.Models.EC2
{
    public class AwsEc2Volume : AwsResource
    {
        public AwsEc2Volume(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsEc2Volume), region)
        {
        }
    }
}