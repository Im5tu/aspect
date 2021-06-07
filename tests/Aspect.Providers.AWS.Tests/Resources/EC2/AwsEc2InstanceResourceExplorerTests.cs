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
    public class AwsEc2InstanceResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsEc2Instance));
        }

        [Fact]
        public async Task ShouldGetAllValuesUsingNextTokens()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeInstancesAsync(It.Is<DescribeInstancesRequest>(x => x.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInstancesResponse
                {
                    NextToken = nameof(DescribeInstancesResponse.NextToken),
                    Reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            Instances = new List<Instance>
                            {
                                new()
                            }
                        }
                    }
                });
            ec2Client.Setup(x => x.DescribeInstancesAsync(It.Is<DescribeInstancesRequest>(x => x.NextToken == nameof(DescribeInstancesResponse.NextToken)), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInstancesResponse
                {
                    Reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            Instances = new List<Instance>
                            {
                                new()
                            }
                        }
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();
            resources.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeInstancesAsync(It.Is<DescribeInstancesRequest>(x => x.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInstancesResponse
                {
                    Reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            Instances = new List<Instance>
                            {
                                new()
                                {
                                    Architecture = nameof(Instance.Architecture),
                                    BootMode = BootModeValues.Uefi,
                                    CpuOptions = new CpuOptions
                                    {
                                        CoreCount = 2,
                                        ThreadsPerCore = 2
                                    },
                                    EbsOptimized = true,
                                    EnaSupport = true,
                                    EnclaveOptions = new EnclaveOptions
                                    {
                                        Enabled = true
                                    },
                                    HibernationOptions = new HibernationOptions
                                    {
                                        Configured = true
                                    },
                                    Hypervisor = nameof(Instance.Hypervisor),
                                    IamInstanceProfile = new IamInstanceProfile
                                    {
                                        Arn = nameof(IamInstanceProfile.Arn),
                                        Id = nameof(IamInstanceProfile.Id)
                                    },
                                    ImageId = nameof(Instance.ImageId),
                                    InstanceId = nameof(Instance.InstanceId),
                                    InstanceLifecycle = InstanceLifecycleType.Scheduled,
                                    InstanceType = InstanceType.A12xlarge,
                                    KernelId = nameof(Instance.KernelId),
                                    KeyName = nameof(Instance.KeyName),
                                    Licenses = new List<LicenseConfiguration>
                                    {
                                        new ()
                                        {
                                            LicenseConfigurationArn = nameof(LicenseConfiguration.LicenseConfigurationArn)
                                        }
                                    },
                                    Monitoring = new Monitoring { State = MonitoringState.Enabled },
                                    NetworkInterfaces = new List<InstanceNetworkInterface>
                                    {
                                        new ()
                                        {
                                            Description = nameof(InstanceNetworkInterface.Description),
                                            InterfaceType = nameof(InstanceNetworkInterface.InterfaceType),
                                            MacAddress = nameof(InstanceNetworkInterface.MacAddress),
                                            NetworkInterfaceId = nameof(InstanceNetworkInterface.NetworkInterfaceId),
                                            OwnerId = nameof(InstanceNetworkInterface.OwnerId),
                                            PrivateDnsName = nameof(InstanceNetworkInterface.PrivateDnsName),
                                            PrivateIpAddress = nameof(InstanceNetworkInterface.PrivateIpAddress),
                                            PrivateIpAddresses = new List<InstancePrivateIpAddress>
                                            {
                                                new InstancePrivateIpAddress { PrivateIpAddress = nameof(InstanceNetworkInterface.PrivateIpAddress) }
                                            },
                                            SourceDestCheck = true,
                                            Status = NetworkInterfaceStatus.Available,
                                            SubnetId = nameof(InstanceNetworkInterface.SubnetId),
                                            VpcId = nameof(InstanceNetworkInterface.VpcId)
                                        }
                                    },
                                    Placement = new Placement
                                    {
                                        Affinity = nameof(Placement.Affinity),
                                        AvailabilityZone = nameof(Placement.AvailabilityZone),
                                        GroupName = nameof(Placement.GroupName),
                                        HostId = nameof(Placement.HostId),
                                        PartitionNumber = 1,
                                        SpreadDomain = nameof(Placement.SpreadDomain),
                                        Tenancy = nameof(Placement.Tenancy),
                                    },
                                    Platform = PlatformValues.Windows,
                                    PrivateDnsName = nameof(Instance.PrivateDnsName),
                                    PrivateIpAddress = nameof(Instance.PrivateIpAddress),
                                    PublicDnsName = nameof(Instance.PublicDnsName),
                                    PublicIpAddress = nameof(Instance.PublicIpAddress),
                                    RamdiskId = nameof(Instance.RamdiskId),
                                    RootDeviceName = nameof(Instance.RootDeviceName),
                                    RootDeviceType = RootDeviceType.InstanceStore.Value,
                                    SourceDestCheck = true,
                                    SriovNetSupport = nameof(Instance.SriovNetSupport),
                                    State = new InstanceState { Name = InstanceStateName.Running, Code = 0},
                                    SubnetId = nameof(Instance.SubnetId),
                                    VirtualizationType = VirtualizationType.Hvm,
                                    Tags = new List<Tag>
                                    {
                                        new("Name", "Test")
                                    },
                                    VpcId = nameof(Instance.VpcId)
                                }
                            }
                        }
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Instance>();
            var sut = (AwsEc2Instance)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Instance));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:instance/InstanceId");
            sut.Region.Should().Be("eu-west-1");
            sut.Architecture.Should().Be(nameof(Instance.Architecture));
            sut.Hypervisor.Should().Be(nameof(Instance.Hypervisor));
            sut.Monitoring.Should().Be("enabled");
            sut.Placement.Should().BeEquivalentTo(new AwsEc2Instance.InstancePlacement
            {
                Affinity = nameof(Placement.Affinity),
                AvailabilityZone = nameof(Placement.AvailabilityZone),
                GroupName = nameof(Placement.GroupName),
                HostId = nameof(Placement.HostId),
                PartitionNumber = 1,
                SpreadDomain = nameof(Placement.SpreadDomain),
                Tenancy = nameof(Placement.Tenancy),
            });
            sut.Platform.Should().Be("Windows");
            sut.State.Should().Be("running");
            sut.BootMode.Should().Be("uefi");
            sut.CpuOptions.Should().BeEquivalentTo(new AwsEc2Instance.Cpu
            {
                Cores = 2,
                Threads = 2
            });
            sut.EbsOptimized.Should().BeTrue();
            sut.EnaSupport.Should().BeTrue();
            sut.EnclaveEnabled.Should().BeTrue();
            sut.HibernationEnabled.Should().BeTrue();
            sut.ImageId.Should().Be(nameof(Instance.ImageId));
            sut.InstanceId.Should().Be(nameof(Instance.InstanceId));
            sut.InstanceLifecycle.Should().Be("scheduled");
            sut.InstanceType.Should().Be(InstanceType.A12xlarge);
            sut.KernelId.Should().Be(nameof(Instance.KernelId));
            sut.KeyName.Should().Be(nameof(Instance.KeyName));
            sut.NetworkInterfaces.Should().BeEquivalentTo(new List<AwsEc2Instance.NetworkInterface>
            {
                new ()
                {
                    Description = nameof(InstanceNetworkInterface.Description),
                    InterfaceType = nameof(InstanceNetworkInterface.InterfaceType),
                    MacAddress = nameof(InstanceNetworkInterface.MacAddress),
                    NetworkInterfaceId = nameof(InstanceNetworkInterface.NetworkInterfaceId),
                    OwnerId = nameof(InstanceNetworkInterface.OwnerId),
                    PrivateDnsName = nameof(InstanceNetworkInterface.PrivateDnsName),
                    PrivateIpAddresses = new List<string>
                    {
                        nameof(InstanceNetworkInterface.PrivateIpAddress)
                    },
                    PrimaryPrivateIpAddress = nameof(InstanceNetworkInterface.PrivateIpAddress),
                    SourceDestCheck = true,
                    Status = NetworkInterfaceStatus.Available,
                    SubnetId = nameof(InstanceNetworkInterface.SubnetId),
                    VpcId = nameof(InstanceNetworkInterface.VpcId)
                }
            });
            sut.RamdiskId.Should().Be(nameof(Instance.RamdiskId));
            sut.SubnetId.Should().Be(nameof(Instance.SubnetId));
            sut.VirtualizationType.Should().Be("hvm");
            sut.IamInstanceProfile.Should().Be("Arn");
            sut.PrivateDnsName.Should().Be(nameof(Instance.PrivateDnsName));
            sut.PrivateIpAddress.Should().Be(nameof(Instance.PrivateIpAddress));
            sut.PublicDnsName.Should().Be(nameof(Instance.PublicDnsName));
            sut.PublicIpAddress.Should().Be(nameof(Instance.PublicIpAddress));
            sut.RootDeviceName.Should().Be(nameof(Instance.RootDeviceName));
            sut.RootDeviceType.Should().Be("instance-store");
            sut.SriovNetSupport.Should().Be(nameof(Instance.SriovNetSupport));
            sut.SourceDestCheck.Should().BeTrue();
        }

        [Fact]
        public async Task ShouldNotFailWhenPropertiesAreNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);

            ec2Client.Setup(x => x.DescribeInstancesAsync(It.Is<DescribeInstancesRequest>(x => x.NextToken == null), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DescribeInstancesResponse
                {
                    Reservations = new List<Reservation>
                    {
                        new Reservation
                        {
                            Instances = new List<Instance>
                            {
                                new()
                                {
                                    Architecture = nameof(Instance.Architecture),
                                    BootMode = BootModeValues.Uefi,
                                    EbsOptimized = true,
                                    EnaSupport = true,
                                    Hypervisor = nameof(Instance.Hypervisor),
                                    ImageId = nameof(Instance.ImageId),
                                    InstanceId = nameof(Instance.InstanceId),
                                    InstanceLifecycle = InstanceLifecycleType.Scheduled,
                                    InstanceType = InstanceType.A12xlarge,
                                    KernelId = nameof(Instance.KernelId),
                                    KeyName = nameof(Instance.KeyName),
                                    Platform = PlatformValues.Windows,
                                    PrivateDnsName = nameof(Instance.PrivateDnsName),
                                    PrivateIpAddress = nameof(Instance.PrivateIpAddress),
                                    PublicDnsName = nameof(Instance.PublicDnsName),
                                    PublicIpAddress = nameof(Instance.PublicIpAddress),
                                    RamdiskId = nameof(Instance.RamdiskId),
                                    RootDeviceName = nameof(Instance.RootDeviceName),
                                    RootDeviceType = RootDeviceType.InstanceStore.Value,
                                    SourceDestCheck = true,
                                    SriovNetSupport = nameof(Instance.SriovNetSupport),
                                    SubnetId = nameof(Instance.SubnetId),
                                    VirtualizationType = VirtualizationType.Hvm,
                                    Tags = new List<Tag>
                                    {
                                        new("Name", "Test")
                                    },
                                    VpcId = nameof(Instance.VpcId)
                                }
                            }
                        }
                    }
                });

            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsEc2Instance>();
            var sut = (AwsEc2Instance)resources[0];
            sut.Type.Should().Be(nameof(AwsEc2Instance));
            sut.Name.Should().Be("Test");
            sut.Arn.Should().Be("arn:aws:ec2:eu-west-1:000000000000:instance/InstanceId");
            sut.Region.Should().Be("eu-west-1");
            sut.Architecture.Should().Be(nameof(Instance.Architecture));
            sut.Hypervisor.Should().Be(nameof(Instance.Hypervisor));
            sut.Monitoring.Should().Be("Unknown");
            sut.Placement.Should().BeEquivalentTo(new AwsEc2Instance.InstancePlacement());
            sut.Platform.Should().Be("Windows");
            sut.State.Should().Be("Unknown");
            sut.BootMode.Should().Be("uefi");
            sut.CpuOptions.Should().BeEquivalentTo(new AwsEc2Instance.Cpu
            {
                Cores = -1,
                Threads = -1
            });
            sut.EbsOptimized.Should().BeTrue();
            sut.EnaSupport.Should().BeTrue();
            sut.EnclaveEnabled.Should().BeFalse();
            sut.HibernationEnabled.Should().BeFalse();
            sut.ImageId.Should().Be(nameof(Instance.ImageId));
            sut.InstanceId.Should().Be(nameof(Instance.InstanceId));
            sut.InstanceLifecycle.Should().Be("scheduled");
            sut.InstanceType.Should().Be(InstanceType.A12xlarge);
            sut.KernelId.Should().Be(nameof(Instance.KernelId));
            sut.KeyName.Should().Be(nameof(Instance.KeyName));
            sut.NetworkInterfaces.Should().BeEquivalentTo(new List<AwsEc2Instance.NetworkInterface>());
            sut.RamdiskId.Should().Be(nameof(Instance.RamdiskId));
            sut.SubnetId.Should().Be(nameof(Instance.SubnetId));
            sut.VirtualizationType.Should().Be("hvm");
            sut.IamInstanceProfile.Should().BeNull();
            sut.PrivateDnsName.Should().Be(nameof(Instance.PrivateDnsName));
            sut.PrivateIpAddress.Should().Be(nameof(Instance.PrivateIpAddress));
            sut.PublicDnsName.Should().Be(nameof(Instance.PublicDnsName));
            sut.PublicIpAddress.Should().Be(nameof(Instance.PublicIpAddress));
            sut.RootDeviceName.Should().Be(nameof(Instance.RootDeviceName));
            sut.RootDeviceType.Should().Be("instance-store");
            sut.SriovNetSupport.Should().Be(nameof(Instance.SriovNetSupport));
            sut.SourceDestCheck.Should().BeTrue();
        }

        private AwsAccount GetAccount() => new AwsAccount(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsEc2InstanceResourceExplorer(_ => ec2Client);
        }
    }
}