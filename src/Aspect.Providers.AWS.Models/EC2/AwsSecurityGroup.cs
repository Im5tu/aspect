using System.Collections.Generic;

namespace Aspect.Providers.AWS.Models.EC2
{
    /// <summary>
    ///     Represents an EC2 security group
    /// </summary>
    public class AwsSecurityGroup : AwsResource
    {
        private readonly List<Rule> _ingressRules = new();
        private readonly List<Rule> _egressRules = new();

        /// <summary>
        ///     The name of the security group
        /// </summary>
        public string? Description { get; internal set; }

        /// <summary>
        ///     The inbound security group rules
        /// </summary>
        public IReadOnlyCollection<Rule> IngressRules => _ingressRules;
        /// <summary>
        ///     The outbound security group rules
        /// </summary>
        public IReadOnlyCollection<Rule> EgressRules => _egressRules;

        /// <inheritDoc />
        public AwsSecurityGroup(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags)
            : base(account, arn, name, tags, nameof(AwsSecurityGroup))
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rule"></param>
        public void AddIngressRule(Rule rule) => _ingressRules.Add(rule);
        /// <summary>
        ///
        /// </summary>
        /// <param name="rule"></param>
        public void AddEgressRule(Rule rule) => _egressRules.Add(rule);

        /// <summary>
        ///     Represents an EC2
        /// </summary>
        public class Rule
        {
            private readonly List<IPV4Entry> _ipv4Cidrs = new();
            private readonly List<IPV6Entry> _ipv6Cidrs = new();
            private readonly List<PrefixEntry> _prefixes = new();
            private readonly List<SecurityGroupEntry> _securityGroups = new();

            /// <summary></summary>
            public int? FromPort { get; set; }
            /// <summary></summary>
            public int? ToPort { get; set; }
            /// <summary></summary>
            public string Protocol { get; }
            /// <summary></summary>
            public IReadOnlyCollection<IPV4Entry> IPV4Ranges => _ipv4Cidrs;
            /// <summary></summary>
            public IReadOnlyCollection<IPV6Entry> IPV6Ranges => _ipv6Cidrs;
            /// <summary></summary>
            public IReadOnlyCollection<PrefixEntry> Prefixes => _prefixes;
            /// <summary></summary>
            public IReadOnlyCollection<SecurityGroupEntry> SecurityGroups => _securityGroups;

            /// <param name="protocol"></param>
            public Rule(string protocol)
            {
                Protocol = protocol;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="cidr"></param>
            /// <param name="description"></param>
            public void AddIpV4Cidr(string cidr, string? description)
                => _ipv4Cidrs.Add(new IPV4Entry(cidr, description));

            /// <summary>
            ///
            /// </summary>
            /// <param name="cidr"></param>
            /// <param name="description"></param>
            public void AddIpV6Cidr(string cidr, string? description)
                => _ipv6Cidrs.Add(new IPV6Entry(cidr, description));

            /// <summary>
            ///
            /// </summary>
            /// <param name="id"></param>
            /// <param name="description"></param>
            public void AddPrefix(string id, string? description)
                => _prefixes.Add(new PrefixEntry(id, description));

            /// <summary>
            ///
            /// </summary>
            /// <param name="id"></param>
            /// <param name="name"></param>
            /// <param name="description"></param>
            /// <param name="account"></param>
            /// <param name="vpcId"></param>
            /// <param name="vpcPeeringConnectionId"></param>
            public void AddSecurityGroup(string id, string? name, string? description, string account, string? vpcId, string? vpcPeeringConnectionId)
                => _securityGroups.Add(new(id, name, description, account, vpcId, vpcPeeringConnectionId));

            #pragma warning disable CS1591
            public record IPV4Entry(string CIDR, string? Description);
            public record IPV6Entry(string CIDR, string? Description);
            public record PrefixEntry(string Id, string? Description);
            public record SecurityGroupEntry(string Id, string? Name, string? Description, string Account, string? VpcId, string? VpcPeeringConnectionId);
        }
    }
}
