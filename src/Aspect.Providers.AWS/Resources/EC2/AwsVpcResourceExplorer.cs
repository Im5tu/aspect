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
    internal sealed class AwsVpcResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsVpcResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsVpc))
        {
            _creator = creator;
        }

        public AwsVpcResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            var detailTasks = new List<Task>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeVpcsAsync(new DescribeVpcsRequest { NextToken = nextToken, Filters = new List<Filter>
                {
                    new() { Name = "owner-id", Values = new() { account.Id.Id } }
                }}, cancellationToken);
                foreach (var vpc in response.Vpcs)
                {
                    var arn = GenerateArn(account, region, "ec2", $"vpc/{vpc.VpcId}");
                    var awsVpc = new AwsVpc(account, arn, vpc.Tags.GetName(), vpc.Tags.Convert(), region.SystemName)
                    {
                        State = vpc.State?.Value.ValueOrEmpty(),
                        CidrBlock = vpc.CidrBlock.ValueOrEmpty(),
                        InstanceTenancy = vpc.InstanceTenancy?.Value.ValueOrEmpty(),
                        IsDefault = vpc.IsDefault,
                        OwnerId = vpc.OwnerId.ValueOrEmpty(),
                        Id = vpc.VpcId.ValueOrEmpty(),
                    };
                    detailTasks.Add(GetVpcDetails(awsVpc, ec2Client, cancellationToken));
                    result.Add(awsVpc);
                }

            } while (!string.IsNullOrWhiteSpace(nextToken));

            await Task.WhenAll(detailTasks);

            return result;
        }

        private async Task GetVpcDetails(AwsVpc awsVpc, IAmazonEC2 ec2Client, CancellationToken cancellationToken)
        {
            var subnetsTask = GetSubnets(awsVpc, ec2Client, cancellationToken);
            var endpointsTask = GetEndpoints(awsVpc, ec2Client, cancellationToken);
            var peeringTask = GetPeeringConnections(awsVpc, ec2Client, cancellationToken);

            await Task.WhenAll(subnetsTask, endpointsTask, peeringTask);
        }

        private async Task GetSubnets(AwsVpc awsVpc, IAmazonEC2 ec2Client, CancellationToken cancellationToken)
        {
            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeSubnetsAsync(new DescribeSubnetsRequest
                {
                    NextToken = nextToken,
                    Filters = new()
                    {
                        new() {Name = "vpc-id", Values = new() {awsVpc.Id}}
                    }
                }, cancellationToken);

                var subnets = new List<AwsVpc.VpcSubnet>();
                awsVpc.Subnets = subnets;
                foreach (var sn in response.Subnets)
                {
                    subnets.Add(new()
                    {
                        AvailabilityZone = sn.AvailabilityZone.ValueOrEmpty(),
                        CidrBlock = sn.CidrBlock.ValueOrEmpty(),
                        OutpostArn = sn.OutpostArn.ValueOrEmpty(),
                        OwnerId = sn.OwnerId.ValueOrEmpty(),
                        Id = sn.SubnetId.ValueOrEmpty(),
                        Arn = sn.SubnetArn.ValueOrEmpty(),
                        IsDefaultForAvailabilityZone = sn.DefaultForAz,
                        AvailableIpAddressCount = sn.AvailableIpAddressCount,
                        MapPublicIpOnLaunch = sn.MapPublicIpOnLaunch,
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));
        }

        private async Task GetEndpoints(AwsVpc awsVpc, IAmazonEC2 ec2Client, CancellationToken cancellationToken)
        {
            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeVpcEndpointsAsync(new DescribeVpcEndpointsRequest
                {
                    NextToken = nextToken,
                    Filters = new()
                    {
                        new() {Name = "vpc-id", Values = new() {awsVpc.Id}}
                    }
                }, cancellationToken);

                var endpoints = new List<AwsVpc.VpcEndpoint>();
                awsVpc.Endpoints = endpoints;
                foreach (var ep in response.VpcEndpoints)
                {
                    endpoints.Add(new()
                    {
                        Id = ep.VpcEndpointId.ValueOrEmpty(),
                        OwnerId = ep.OwnerId?.ValueOrEmpty(),
                        State = ep.State?.Value.ValueOrEmpty(),
                        DnsEntries = ep.DnsEntries?.Select(x => x.DnsName).ValueOrEmpty(),
                        ServiceName = ep.ServiceName.ValueOrEmpty(),
                        RequesterManaged = ep.RequesterManaged,
                        SubnetIds = ep.SubnetIds.ValueOrEmpty(),
                        NetworkInterfaces = ep.NetworkInterfaceIds.ValueOrEmpty(),
                        PrivateDnsEnabled = ep.PrivateDnsEnabled,
                        RouteTables = ep.RouteTableIds.ValueOrEmpty(),
                        Type = ep.VpcEndpointType?.Value.ValueOrEmpty(),
                        SecurityGroups = ep.Groups?.Select(x => x.GroupId).ValueOrEmpty(),
                    });
                }

            } while (!string.IsNullOrWhiteSpace(nextToken));
        }

        private async Task GetPeeringConnections(AwsVpc awsVpc, IAmazonEC2 ec2Client, CancellationToken cancellationToken)
        {
            var result = new List<AwsVpc.VpcPeeringConnection>();
            awsVpc.PeeringConnections = result;

            string? nextToken = null;
            do
            {
                var response1 = await ec2Client.DescribeVpcPeeringConnectionsAsync(new DescribeVpcPeeringConnectionsRequest
                {
                    NextToken = nextToken,
                    Filters = new()
                    {
                        new () { Name = "accepter-vpc-info.vpc-id", Values = new () { awsVpc.Id }}
                    }
                }, cancellationToken);

                var response2 = await ec2Client.DescribeVpcPeeringConnectionsAsync(new DescribeVpcPeeringConnectionsRequest
                {
                    NextToken = nextToken,
                    Filters = new()
                    {
                        new () { Name = "requester-vpc-info.vpc-id", Values = new () { awsVpc.Id }}
                    }
                }, cancellationToken);

                foreach (var el in response1.VpcPeeringConnections.Concat(response2.VpcPeeringConnections))
                {
                    result.Add(new ()
                    {
                        Accepter = el.AccepterVpcInfo?.VpcId?.ValueOrEmpty(),
                        Requester = el.RequesterVpcInfo?.VpcId?.ValueOrEmpty(),
                        Status = el.Status?.Code?.Value.ValueOrEmpty(),
                        Id = el.VpcPeeringConnectionId.ValueOrEmpty()
                    });
                }

            } while (!string.IsNullOrWhiteSpace(nextToken));
        }
    }
}
