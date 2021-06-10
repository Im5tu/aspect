using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("A security group acts as a virtual firewall for your instance to control inbound and outbound traffic. A security group specifies the actions that are allowed, not the actions that are blocked.")]
    public class AwsSecurityGroup : AwsResource
    {
        private readonly List<Rule> _ingressRules = new();
        private readonly List<Rule> _egressRules = new();

        [Description("A friendly description of what the security group allows.")]
        public string? Description { get; init; }

        [Description("A series of rules that affect inbound traffic.")]
        public IReadOnlyCollection<Rule> IngressRules => _ingressRules;
        [Description("A series of rules that affect outbound traffic.")]
        public IReadOnlyCollection<Rule> EgressRules => _egressRules;

        [Description("The ID of the security group.")]
        public string? Id { get; init; }

        [Description("The AWS account ID of the owner of the security group.")]
        public string? OwnerId { get; init; }

        [Description("The ID of the VPC for the security group.")]
        public string? VpcId { get; init; }

        public AwsSecurityGroup(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsSecurityGroup), region)
        {
        }

        public void AddIngressRule(Rule rule) => _ingressRules.Add(rule);
        public void AddEgressRule(Rule rule) => _egressRules.Add(rule);

        protected override string FormatProperty(string propertyName)
        {
            return propertyName switch
            {
                nameof(Description) => Description ?? string.Empty,
                nameof(IngressRules) => string.Join(Environment.NewLine, IngressRules.Select(x => $"- {x.ToString()}")),
                nameof(EgressRules) => string.Join(Environment.NewLine, EgressRules.Select(x => $"- {x.ToString()}")),
                _ => base.FormatProperty(propertyName)
            };
        }

        [Description("Describes a set of permissions for a security group.")]
        public class Rule
        {
            private readonly List<IPV4Entry> _ipv4Cidrs = new();
            private readonly List<IPV6Entry> _ipv6Cidrs = new();
            private readonly List<PrefixEntry> _prefixes = new();
            private readonly List<SecurityGroupEntry> _securityGroups = new();

            [Description("The start of port range for the TCP and UDP protocols, or an ICMP/ICMPv6 type number. A value of -1 indicates all ICMP/ICMPv6 types.")]
            public int? FromPort { get; set; }
            [Description("The end of port range for the TCP and UDP protocols, or an ICMP/ICMPv6 type number. A value of -1 indicates all ICMP/ICMPv6 types.")]
            public int? ToPort { get; set; }
            [Description("The protocol to allow. The most common protocols are 6 (TCP), 17 (UDP), and 1 (ICMP). -1 indicates all protocols.")]
            public string Protocol { get; }
            [Description("A collection of IPV4 CIDR ranges that are allowed to communicate with this security group.")]
            public IReadOnlyCollection<IPV4Entry> IPV4Ranges => _ipv4Cidrs;
            [Description("A collection of IPV6 CIDR ranges that are allowed to communicate with this security group.")]
            public IReadOnlyCollection<IPV6Entry> IPV6Ranges => _ipv6Cidrs;
            [Description("A prefix list is a set of one or more CIDR blocks. You can use prefix lists to make it easier to configure and maintain your security groups and route tables. You can create a prefix list from the IP addresses that you frequently use, and reference them as a set in security group rules and routes instead of referencing them individually.")]
            public IReadOnlyCollection<PrefixEntry> Prefixes => _prefixes;
            [Description("A collection of other security groups that are allowed to communicate with this security group.")]
            public IReadOnlyCollection<SecurityGroupEntry> SecurityGroups => _securityGroups;

            public Rule(string protocol)
            {
                Protocol = protocol;
            }

            public void AddIpV4Cidr(string cidr, string? description)
                => _ipv4Cidrs.Add(new IPV4Entry(cidr, description));
            public void AddIpV6Cidr(string cidr, string? description)
                => _ipv6Cidrs.Add(new IPV6Entry(cidr, description));
            public void AddPrefix(string id, string? description)
                => _prefixes.Add(new PrefixEntry(id, description));
            public void AddSecurityGroup(string id, string? name, string? description, string account, string? vpcId, string? vpcPeeringConnectionId)
                => _securityGroups.Add(new(id, name, description, account, vpcId, vpcPeeringConnectionId));

            [Description("Describes a IPV4 CIDR range that is allowed to talk to the defined security group.")]
            public record IPV4Entry([property: Description("The IPV4 classless inter-domain routing (CIDR) range associated with this entry.")] string CIDR, [property: Description("A friendly description of what the CIDR range belongs to.")] string? Description);

            [Description("Describes a IPV6 CIDR range that is allowed to talk to the defined security group.")]
            public record IPV6Entry([property: Description("The IPV6 classless inter-domain routing (CIDR) range associated with this entry.")] string CIDR, [property: Description("A friendly description of what the CIDR range belongs to.")] string? Description);

            [Description("Describes a prefix list that is allowed to talk to the defined security group.")]
            public record PrefixEntry([property: Description("The id of the prefix entry.")] string Id, [property: Description("A friendly description of what the prefix entry belongs to.")] string? Description);

            [Description(@"Describes a security group that is allowed to talk to the defined security group.")]
            public record SecurityGroupEntry([property: Description("The id of the security group.")]string Id,
                [property: Description("The name of the security group")] string? Name,
                [property: Description("A friendly description of what the security group does.")] string? Description,
                [property: Description("The account that the security group is associated with.")] string Account,
                [property: Description("The VPC that the security group is associated with.")] string? VpcId,
                [property: Description("The VPC peering connection the security group is associated with.")] string? VpcPeeringConnectionId);
        }
    }
}
