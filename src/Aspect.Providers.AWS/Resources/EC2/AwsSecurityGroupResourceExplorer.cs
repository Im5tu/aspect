using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsSecurityGroupResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsSecurityGroupResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsSecurityGroup))
        {
            _creator = creator;
        }

        public AwsSecurityGroupResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config {RegionEndpoint = region});

            var groups = new List<AwsSecurityGroup>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeSecurityGroupsAsync(new DescribeSecurityGroupsRequest
                {
                    NextToken = nextToken,
                    Filters = new List<Filter>
                    {
                        new() { Name = "owner-id", Values = new() { account.Id.Id } }
                    }
                }, cancellationToken);

                nextToken = response.NextToken;
                foreach (var sg in response.SecurityGroups)
                {
                    // sg.Description
                    // Inbound: sg.IpPermissions
                    // Egress:  sg.IpPermissionsEgress

                    var arn = GenerateArn(account, region, "ec2", $"security-group/{sg.GroupId}");
                    var securityGroup = new AwsSecurityGroup(account, arn, sg.GroupName.ValueOrEmpty(), sg.Tags.Convert(), region.SystemName)
                    {
                        Id = sg.GroupId.ValueOrEmpty(),
                        OwnerId = sg.OwnerId.ValueOrEmpty(),
                        VpcId = sg.VpcId.ValueOrEmpty(),
                        Description = sg.Description.ValueOrEmpty()
                    };

                    foreach (var inboundRule in sg.IpPermissions)
                        securityGroup.AddIngressRule(ParseSecurityGroupRule(inboundRule));

                    foreach (var outboundRule in sg.IpPermissions)
                        securityGroup.AddEgressRule(ParseSecurityGroupRule(outboundRule));

                    groups.Add(securityGroup);
                }
            } while (nextToken is { });

            return groups;
        }

        private static AwsSecurityGroup.Rule ParseSecurityGroupRule(IpPermission permission)
        {
            var rule = new AwsSecurityGroup.Rule(permission.IpProtocol);

            if (permission.FromPort != 0)
                rule.FromPort = permission.FromPort;

            if (permission.ToPort != 0)
                rule.ToPort = permission.ToPort;

            foreach (var ipv4Range in permission.Ipv4Ranges ?? Enumerable.Empty<IpRange>())
                rule.AddIpV4Cidr(ipv4Range.CidrIp, ipv4Range.Description);

            foreach (var ipv6Range in permission.Ipv6Ranges ?? Enumerable.Empty<Ipv6Range>())
                rule.AddIpV6Cidr(ipv6Range.CidrIpv6, ipv6Range.Description);

            foreach (var prefix in permission.PrefixListIds ?? Enumerable.Empty<PrefixListId>())
                rule.AddPrefix(prefix.Id, prefix.Description);

            foreach (var sg in permission.UserIdGroupPairs ?? Enumerable.Empty<UserIdGroupPair>())
                rule.AddSecurityGroup(sg.GroupId, sg.GroupName, sg.Description, sg.UserId, sg.VpcId, sg.VpcPeeringConnectionId);

            return rule;
        }
    }
}
