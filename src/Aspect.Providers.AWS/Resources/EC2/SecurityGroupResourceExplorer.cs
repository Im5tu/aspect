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
    internal sealed class SecurityGroupResourceExplorer : AWSResourceExplorer
    {
        public SecurityGroupResourceExplorer()
            : base(typeof(AwsSecurityGroup))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = new AmazonEC2Client(new AmazonEC2Config { RegionEndpoint = region });
            try
            {
                var groups = new List<AwsSecurityGroup>();

                string? nextToken = null;
                do
                {
                    var response = await ec2Client.DescribeSecurityGroupsAsync(new DescribeSecurityGroupsRequest
                    {
                        NextToken = nextToken
                    }, cancellationToken);

                    nextToken = response.NextToken;
                    foreach (var sg in response.SecurityGroups)
                    {
                        // sg.Description
                        // Inbound: sg.IpPermissions
                        // Egress:  sg.IpPermissionsEgress

                        var arn = $"arn:aws:ec2:{ec2Client.Config.RegionEndpoint.SystemName}:{account.Id.Id}:security-group/{sg.GroupId}";
                        var securityGroup = new AwsSecurityGroup(account, arn, sg.GroupName, sg.Tags.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList(), region.SystemName);

                        foreach (var inboundRule in sg.IpPermissions)
                            securityGroup.AddIngressRule(ParseSecurityGroupRule(inboundRule));

                        foreach (var outboundRule in sg.IpPermissions)
                            securityGroup.AddEgressRule(ParseSecurityGroupRule(outboundRule));

                        groups.Add(securityGroup);
                    }
                } while (nextToken is { });

                return groups;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static AwsSecurityGroup.Rule ParseSecurityGroupRule(IpPermission permission)
        {
            var rule = new AwsSecurityGroup.Rule(permission.IpProtocol);

            if (permission.FromPort != 0)
                rule.FromPort = permission.FromPort;

            if (permission.ToPort != 0)
                rule.ToPort = permission.ToPort;

            foreach (var ipv4Range in permission.Ipv4Ranges)
                rule.AddIpV4Cidr(ipv4Range.CidrIp, ipv4Range.Description);

            foreach (var ipv6Range in permission.Ipv6Ranges)
                rule.AddIpV6Cidr(ipv6Range.CidrIpv6, ipv6Range.Description);

            foreach (var prefix in permission.PrefixListIds)
                rule.AddPrefix(prefix.Id, prefix.Description);

            foreach (var sg in permission.UserIdGroupPairs)
                rule.AddSecurityGroup(sg.GroupId, sg.GroupName, sg.Description, sg.UserId, sg.VpcId, sg.VpcPeeringConnectionId);

            return rule;
        }
    }
}
