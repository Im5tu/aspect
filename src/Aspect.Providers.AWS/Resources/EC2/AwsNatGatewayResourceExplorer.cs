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
    internal sealed class AwsNatGatewayResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsNatGatewayResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsNatGateway))
        {
            _creator = creator;
        }

        public AwsNatGatewayResourceExplorer()
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
                var response = await ec2Client.DescribeNatGatewaysAsync(new DescribeNatGatewaysRequest { NextToken = nextToken }, cancellationToken);
                nextToken = response.NextToken;

                foreach (var gw in response.NatGateways)
                {
                    var arn = GenerateArn(account, region, "ec2", $"nat-gateway/{gw.NatGatewayId}");
                    result.Add(new AwsNatGateway(account, arn, gw.Tags.GetName(), gw.Tags.Convert(), region.SystemName)
                    {
                        Id = gw.NatGatewayId,
                        State = gw.State?.Value,
                        FailureCode = gw.FailureCode,
                        ProvisionedBandwidth = gw.ProvisionedBandwidth?.Provisioned,
                        SubnetId = gw.SubnetId,
                        VpcId = gw.VpcId,
                        Addresses = Map(gw.NatGatewayAddresses)
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }

        private IEnumerable<AwsNatGateway.Address> Map(List<NatGatewayAddress>? gwNatGatewayAddresses)
        {
            var result = new List<AwsNatGateway.Address>();

            foreach (var address in gwNatGatewayAddresses ?? Enumerable.Empty<NatGatewayAddress>())
                result.Add(new AwsNatGateway.Address
                {
                    AllocationId = address.AllocationId,
                    NetworkInterfaceId = address.NetworkInterfaceId,
                    PrivateIp = address.PrivateIp,
                    PublicIp = address.PublicIp,
                });

            return result;
        }
    }
}
