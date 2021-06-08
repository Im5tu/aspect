using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("A prefix list is a set of one or more CIDR blocks. You can use prefix lists to make it easier to configure and maintain your security groups and route tables.")]
    public class AwsPrefixList : AwsResource
    {
        [Description("The ID of the prefix list.")]
        public string? Id { get; init; }

        [Description("Information about the prefix list entries.")]
        public IEnumerable<string>? Cidrs { get; set; }

        [Description("The IP address version.")]
        public string? AddressFamily { get; set; }

        [Description("The ID of the owner of the prefix list.")]
        public string? OwnerId { get; set; }

        [Description("The state of the prefix list.")]
        public string? State { get; set; }

        public AwsPrefixList(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsPrefixList), region)
        {
        }
    }
}
