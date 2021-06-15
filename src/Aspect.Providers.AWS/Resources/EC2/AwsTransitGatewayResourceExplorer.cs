using System;
using System.Collections.Generic;
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
    internal sealed class AwsTransitGatewayResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsTransitGatewayResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsTransitGateway))
        {
            _creator = creator;
        }

        public AwsTransitGatewayResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeTransitGatewaysAsync(new DescribeTransitGatewaysRequest { NextToken = nextToken, Filters = new List<Filter>
                {
                    new() { Name = "owner-id", Values = new() { account.Id.Id } }
                } }, cancellationToken);
                nextToken = response.NextToken;

                foreach (var tg in response.TransitGateways)
                {
                    result.Add(new AwsTransitGateway(account, tg.TransitGatewayArn, tg.Tags.GetName(), tg.Tags.Convert(), region.SystemName)
                    {
                        Id = tg.TransitGatewayId.ValueOrEmpty(),
                        Description = tg.Description.ValueOrEmpty(),
                        Configuration = Map(tg.Options),
                        State = tg.State?.Value?.ValueOrEmpty(),
                        OwnerId = tg.OwnerId.ValueOrEmpty()
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            await Task.Yield();

            return result;
        }

        private static AwsTransitGateway.Config Map(TransitGatewayOptions tgo)
        {
            return new()
            {
                SupportsDNS = "enable".Equals(tgo?.DnsSupport?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                SupportsMulticast = "enable".Equals(tgo?.MulticastSupport?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                AmazonAsn = tgo?.AmazonSideAsn ?? -1,
                SupportsECMP = "enable".Equals(tgo?.VpnEcmpSupport?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                AutoAcceptsSharedAttachments = "enable".Equals(tgo?.AutoAcceptSharedAttachments?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                DefaultRouteTableAssociation = "enable".Equals(tgo?.DefaultRouteTableAssociation?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                DefaultRouteTablePropagation = "enable".Equals(tgo?.DefaultRouteTablePropagation?.Value.ValueOrEmpty(), StringComparison.OrdinalIgnoreCase),
                TransitGatewayCidrBlocks = tgo?.TransitGatewayCidrBlocks.ValueOrEmpty(),
                AssociationDefaultRouteTableId = tgo?.AssociationDefaultRouteTableId.ValueOrEmpty(),
                PropagationDefaultRouteTableId = tgo?.PropagationDefaultRouteTableId.ValueOrEmpty(),
            };
        }
    }
}
