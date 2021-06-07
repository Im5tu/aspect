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
                            Architecture = instance.Architecture?.Value,
                            BootMode = instance.BootMode?.Value,
                            CpuOptions = Map(instance.CpuOptions),
                            EbsOptimized = instance.EbsOptimized,
                            EnaSupport = instance.EnaSupport,
                            EnclaveEnabled = instance.EnclaveOptions?.Enabled ?? false,
                            HibernationEnabled = instance.HibernationOptions?.Configured ?? false,
                            Hypervisor = instance.Hypervisor?.Value,
                            IamInstanceProfile = instance.IamInstanceProfile?.Arn,
                            ImageId = instance.ImageId,
                            InstanceId = instance.InstanceId,
                            InstanceLifecycle = instance.InstanceLifecycle?.Value,
                            InstanceType = instance.InstanceType?.Value,
                            KernelId = instance.KernelId,
                            KeyName = instance.KeyName,
                            Licenses = instance.Licenses?.Select(x => x.LicenseConfigurationArn)?.ToList(),
                            Monitoring = instance.Monitoring?.State ?? "Unknown",
                            NetworkInterfaces = Map(instance.NetworkInterfaces),
                            Placement = Map(instance.Placement),
                            Platform = instance.Platform?.Value,
                            PrivateDnsName = instance.PrivateDnsName,
                            PrivateIpAddress = instance.PrivateIpAddress,
                            PublicDnsName = instance.PublicDnsName,
                            PublicIpAddress = instance.PublicIpAddress,
                            RamdiskId = instance.RamdiskId,
                            RootDeviceName = instance.RootDeviceName,
                            RootDeviceType = instance.RootDeviceType?.Value,
                            SourceDestCheck = instance.SourceDestCheck,
                            SriovNetSupport = instance.SriovNetSupport,
                            State = instance.State?.Name?.Value ?? "Unknown",
                            SubnetId = instance.SubnetId,
                            VirtualizationType = instance.VirtualizationType?.Value,
                            VpcId = instance.VpcId,
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
                result.Add(new()
                {
                    Description = ni.Description,
                    InterfaceType = ni.InterfaceType,
                    MacAddress = ni.MacAddress,
                    NetworkInterfaceId = ni.NetworkInterfaceId,
                    OwnerId = ni.OwnerId,
                    PrimaryPrivateIpAddress = ni.PrivateIpAddress,
                    PrivateDnsName = ni.PrivateDnsName,
                    PrivateIpAddresses = ni.PrivateIpAddresses?.Select(x => x.PrivateIpAddress)?.ToList(),
                    SourceDestCheck = ni.SourceDestCheck,
                    Status = ni.Status?.Value,
                    SubnetId = ni.SubnetId,
                    VpcId = ni.VpcId,
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
                Affinity = placement?.Affinity,
                AvailabilityZone = placement?.AvailabilityZone,
                GroupName = placement?.GroupName,
                HostId = placement?.HostId,
                PartitionNumber = placement?.PartitionNumber,
                SpreadDomain = placement?.SpreadDomain,
                Tenancy = placement?.Tenancy?.Value,
            };
        }
    }
}
