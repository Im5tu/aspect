using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;
using Aspect.Providers.AWS.Resources.EC2;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aspect.Providers.AWS.Tests.Resources.EC2
{
    public class AwsRouteTableResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsRouteTable));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeRouteTablesAsync(It.IsAny<DescribeRouteTablesRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeRouteTablesResponse
                {
                    RouteTables = new List<RouteTable>()
                    {
                        new()
                        {
                            Associations = new List<RouteTableAssociation>()
                            {
                                new()
                                {
                                    Main = true,
                                    AssociationState = new RouteTableAssociationState { State = RouteTableAssociationStateCode.Associated },
                                    GatewayId = nameof(RouteTableAssociation.GatewayId),
                                    SubnetId = nameof(RouteTableAssociation.SubnetId),
                                    RouteTableId = nameof(RouteTableAssociation.RouteTableId),
                                    RouteTableAssociationId = nameof(RouteTableAssociation.RouteTableAssociationId),
                                }
                            },
                            Routes = new List<Route>()
                            {
                                new ()
                                {
                                    Origin = RouteOrigin.CreateRoute,
                                    State = RouteState.Active,
                                    GatewayId = nameof(Route.GatewayId),
                                    InstanceId = nameof(Route.InstanceId),
                                    CarrierGatewayId = nameof(Route.CarrierGatewayId),
                                    DestinationCidrBlock = nameof(Route.DestinationCidrBlock),
                                    InstanceOwnerId = nameof(Route.InstanceOwnerId),
                                    NatGatewayId = nameof(Route.NatGatewayId),
                                    LocalGatewayId = nameof(Route.LocalGatewayId),
                                    NetworkInterfaceId = nameof(Route.NetworkInterfaceId),
                                    TransitGatewayId = nameof(Route.TransitGatewayId),
                                    DestinationIpv6CidrBlock = nameof(Route.DestinationIpv6CidrBlock),
                                    DestinationPrefixListId = nameof(Route.DestinationPrefixListId),
                                    VpcPeeringConnectionId = nameof(Route.VpcPeeringConnectionId),
                                    EgressOnlyInternetGatewayId = nameof(Route.EgressOnlyInternetGatewayId),
                                }
                            },
                            PropagatingVgws = new List<PropagatingVgw>
                            {
                                new() { GatewayId = nameof(PropagatingVgw.GatewayId) }
                            },
                            OwnerId = nameof(RouteTable.OwnerId),
                            VpcId = nameof(RouteTable.VpcId),
                            RouteTableId = nameof(RouteTable.RouteTableId),
                            Tags = new List<Tag>
                            {
                                new () { Key = "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsRouteTable>();
            var sut = (AwsRouteTable)resources[0];
            sut.Type.Should().Be(nameof(AwsRouteTable));
            sut.Name.Should().Be("Test");
            sut.CloudId.Should().Be("arn:aws:ec2:eu-west-1:000000000000:route-table/RouteTableId");
            sut.Region.Should().Be("eu-west-1");

            sut.Associations.Should().HaveCount(1);
            sut.Routes.Should().HaveCount(1);
            sut.PropagatingVirtualGateways.Should().HaveCount(1);
            sut.OwnerId.Should().Be(nameof(RouteTable.OwnerId));
            sut.VpcId.Should().Be(nameof(RouteTable.VpcId));
            sut.Id.Should().Be(nameof(RouteTable.RouteTableId));
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsRouteTableResourceExplorer(_ => ec2Client);
        }
    }
}
