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
    internal sealed class AwsRouteTableResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsRouteTableResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsRouteTable))
        {
            _creator = creator;
        }

        public AwsRouteTableResourceExplorer()
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
                var response = await ec2Client.DescribeRouteTablesAsync(new DescribeRouteTablesRequest { NextToken = nextToken }, cancellationToken);
                nextToken = response.NextToken;

                foreach (var rt in response.RouteTables)
                {
                    var arn = GenerateArn(account, region, "ec2", $"route-table/{rt.RouteTableId}");
                    result.Add(new AwsRouteTable(account, arn, rt.Tags.GetName(), rt.Tags.Convert(), region.SystemName)
                    {
                        Associations = Map(rt.Associations).ValueOrEmpty(),
                        Routes = Map(rt.Routes).ValueOrEmpty(),
                        OwnerId = rt.OwnerId.ValueOrEmpty(),
                        PropagatingVirtualGateways = rt.PropagatingVgws?.Select(x => x.GatewayId)?.ToList().ValueOrEmpty(),
                        VpcId = rt.VpcId.ValueOrEmpty(),
                        Id = rt.RouteTableId.ValueOrEmpty()
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));


            return result;
        }

        private IEnumerable<AwsRouteTable.Route>? Map(List<Route> routes)
        {
            var result = new List<AwsRouteTable.Route>();

            foreach (var route in routes)
                result.Add(new AwsRouteTable.Route
                {
                    Origin = route.Origin?.Value.ValueOrEmpty(),
                    State = route.State?.Value.ValueOrEmpty(),
                    GatewayId = route.GatewayId?.ValueOrEmpty(),
                    NatInstanceId = route.InstanceId?.ValueOrEmpty(),
                    CarrierGatewayId = route.CarrierGatewayId?.ValueOrEmpty(),
                    DestinationCidrBlock = route.DestinationCidrBlock?.ValueOrEmpty(),
                    NatInstanceOwnerId = route.InstanceOwnerId?.ValueOrEmpty(),
                    LocalGatewayId = route.LocalGatewayId?.ValueOrEmpty(),
                    NatGatewayId = route.NatGatewayId?.ValueOrEmpty(),
                    NetworkInterfaceId = route.NetworkInterfaceId?.ValueOrEmpty(),
                    TransitGatewayId = route.TransitGatewayId?.ValueOrEmpty(),
                    DestinationIpv6CidrBlock = route.DestinationIpv6CidrBlock?.ValueOrEmpty(),
                    DestinationPrefixListId = route.DestinationPrefixListId?.ValueOrEmpty(),
                    VpcPeeringConnectionId = route.VpcPeeringConnectionId?.ValueOrEmpty(),
                    EgressOnlyInternetGatewayId = route.EgressOnlyInternetGatewayId?.ValueOrEmpty(),
                });

            return result;
        }

        private IEnumerable<AwsRouteTable.Association>? Map(List<RouteTableAssociation> associations)
        {
            var result = new List<AwsRouteTable.Association>();

            foreach (var assoc in associations)
                result.Add(new AwsRouteTable.Association
                {
                    IsPrimaryRouteTable = assoc.Main,
                    State = assoc.AssociationState?.State?.Value.ValueOrEmpty(),
                    GatewayId = assoc.GatewayId.ValueOrEmpty(),
                    SubnetId = assoc.SubnetId.ValueOrEmpty(),
                    RouteTableId = assoc.RouteTableId.ValueOrEmpty(),
                    Id = assoc.RouteTableAssociationId.ValueOrEmpty(),
                });

            return result;
        }
    }
}
