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
    internal sealed class AwsEc2InstanceResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsEc2InstanceResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsEc2Instance))
        {
            _creator = creator;
        }

        public AwsEc2InstanceResourceExplorer()
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
                var response = await ec2Client.DescribeInstancesAsync(new DescribeInstancesRequest { NextToken = nextToken }, cancellationToken);
                nextToken = response.NextToken;

                foreach (var reservation in response.Reservations ?? Enumerable.Empty<Reservation>())
                {
                    foreach (var instance in reservation.Instances ?? Enumerable.Empty<Instance>())
                    {
                        var arn = GenerateArn(account, region, "ec2", $"instance/{instance.InstanceId}");
                        result.Add(new AwsEc2Instance(account, arn, instance.Tags.GetName(), instance.Tags.Convert(), region.SystemName)
                        {
                            Architecture = instance.Architecture?.Value.ValueOrEmpty(),
                            BootMode = instance.BootMode?.Value.ValueOrEmpty(),
                            CpuOptions = Map(instance.CpuOptions),
                            EbsOptimized = instance.EbsOptimized,
                            EnaSupport = instance.EnaSupport,
                            EnclaveEnabled = instance.EnclaveOptions?.Enabled ?? false,
                            HibernationEnabled = instance.HibernationOptions?.Configured ?? false,
                            Hypervisor = instance.Hypervisor?.Value.ValueOrEmpty(),
                            IamInstanceProfile = instance.IamInstanceProfile?.Arn.ValueOrEmpty(),
                            ImageId = instance.ImageId.ValueOrEmpty(),
                            InstanceId = instance.InstanceId.ValueOrEmpty(),
                            InstanceLifecycle = instance.InstanceLifecycle?.Value.ValueOrEmpty(),
                            InstanceType = instance.InstanceType?.Value.ValueOrEmpty(),
                            KernelId = instance.KernelId.ValueOrEmpty(),
                            KeyName = instance.KeyName.ValueOrEmpty(),
                            Licenses = instance.Licenses?.Select(x => x.LicenseConfigurationArn).ValueOrEmpty(),
                            Monitoring = instance.Monitoring?.State ?? "Unknown",
                            NetworkInterfaces = Map(instance.NetworkInterfaces),
                            Placement = Map(instance.Placement),
                            Platform = instance.Platform?.Value.ValueOrEmpty(),
                            PrivateDnsName = instance.PrivateDnsName.ValueOrEmpty(),
                            PrivateIpAddress = instance.PrivateIpAddress.ValueOrEmpty(),
                            PublicDnsName = instance.PublicDnsName.ValueOrEmpty(),
                            PublicIpAddress = instance.PublicIpAddress.ValueOrEmpty(),
                            RamdiskId = instance.RamdiskId.ValueOrEmpty(),
                            RootDeviceName = instance.RootDeviceName.ValueOrEmpty(),
                            RootDeviceType = instance.RootDeviceType?.Value.ValueOrEmpty(),
                            SourceDestCheck = instance.SourceDestCheck,
                            SriovNetSupport = instance.SriovNetSupport.ValueOrEmpty(),
                            State = instance.State?.Name?.Value ?? "Unknown",
                            SubnetId = instance.SubnetId.ValueOrEmpty(),
                            VirtualizationType = instance.VirtualizationType?.Value.ValueOrEmpty(),
                            VpcId = instance.VpcId.ValueOrEmpty(),
                        });
                    }
                }
            } while (nextToken is { });

            return result;
        }

        private static IEnumerable<AwsEc2Instance.NetworkInterface> Map(List<InstanceNetworkInterface>? interfaces)
        {
            var result = new List<AwsEc2Instance.NetworkInterface>(interfaces?.Count ?? 0);

            foreach (var ni in interfaces ?? Enumerable.Empty<InstanceNetworkInterface>())
            {
                result.Add(new AwsEc2Instance.NetworkInterface()
                {
                    Description = ni.Description.ValueOrEmpty(),
                    InterfaceType = ni.InterfaceType.ValueOrEmpty(),
                    MacAddress = ni.MacAddress.ValueOrEmpty(),
                    NetworkInterfaceId = ni.NetworkInterfaceId.ValueOrEmpty(),
                    OwnerId = ni.OwnerId.ValueOrEmpty(),
                    PrimaryPrivateIpAddress = ni.PrivateIpAddress.ValueOrEmpty(),
                    PrivateDnsName = ni.PrivateDnsName.ValueOrEmpty(),
                    PrivateIpAddresses = ni.PrivateIpAddresses?.Select(x => x.PrivateIpAddress).ValueOrEmpty(),
                    SourceDestCheck = ni.SourceDestCheck,
                    Status = ni.Status?.Value.ValueOrEmpty(),
                    SubnetId = ni.SubnetId.ValueOrEmpty(),
                    VpcId = ni.VpcId.ValueOrEmpty()
                });
            }

            return result;
        }

        private static AwsEc2Instance.Cpu Map(CpuOptions options)
        {
            return new()
            {
                Cores = options?.CoreCount ?? -1,
                Threads = options?.ThreadsPerCore ?? -1
            };
        }

        private static AwsEc2Instance.InstancePlacement Map(Placement placement)
        {
            return new()
            {
                Affinity = placement?.Affinity.ValueOrEmpty(),
                AvailabilityZone = placement?.AvailabilityZone.ValueOrEmpty(),
                GroupName = placement?.GroupName.ValueOrEmpty(),
                HostId = placement?.HostId.ValueOrEmpty(),
                PartitionNumber = placement?.PartitionNumber,
                SpreadDomain = placement?.SpreadDomain.ValueOrEmpty(),
                Tenancy = placement?.Tenancy?.Value.ValueOrEmpty(),
            };
        }
    }
}
