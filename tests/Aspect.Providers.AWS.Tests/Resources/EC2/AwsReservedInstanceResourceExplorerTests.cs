using System;
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
    public class AwsReservedInstanceResourceExplorerTests
    {
        [Fact]
        public void ShouldHaveCorrectType()
        {
            GetTarget(out _).ResourceType.Should().Be(typeof(AwsReservedInstance));
        }

        [Fact]
        public async Task ShouldMapAllPropertiesWhenNotNull()
        {
            var account = GetAccount();
            var target = GetTarget(out var ec2Client);
            ec2Client.Setup(x => x.DescribeReservedInstancesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new DescribeReservedInstancesResponse
                {
                    ReservedInstances = new List<ReservedInstances>
                    {
                        new()
                        {
                            AvailabilityZone = nameof(ReservedInstances.AvailabilityZone),
                            CurrencyCode = nameof(ReservedInstances.CurrencyCode),
                            End = DateTime.UtcNow,
                            FixedPrice = 100,
                            ReservedInstancesId = nameof(ReservedInstances.ReservedInstancesId),
                            InstanceCount = 100,
                            InstanceTenancy = nameof(ReservedInstances.InstanceTenancy),
                            InstanceType = nameof(ReservedInstances.InstanceType),
                            OfferingClass = nameof(ReservedInstances.OfferingClass),
                            State = nameof(ReservedInstances.State),
                            UsagePrice = 100,
                            Tags = new List<Tag>
                            {
                                new() { Key= "Name", Value = "Test" }
                            }
                        }
                    }
                });
            var resources = (await target.DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();

            resources.Should().HaveCount(1).And.Subject.First().Should().BeOfType<AwsReservedInstance>();
            var sut = (AwsReservedInstance)resources[0];
            sut.Type.Should().Be(nameof(AwsReservedInstance));
            sut.Name.Should().Be("Test");
            sut.CloudId.Should().Be("arn:aws:ec2:eu-west-1:000000000000:reserved-instances/ReservedInstancesId");
            sut.Region.Should().Be("eu-west-1");

            sut.AvailabilityZone.Should().Be(nameof(ReservedInstances.AvailabilityZone));
            sut.CurrencyCode.Should().Be(nameof(ReservedInstances.CurrencyCode));
            sut.Expires.Should().BeCloseTo(DateTime.UtcNow, 1000);
            sut.FixedPrice.Should().Be(100);
            sut.Id.Should().Be(nameof(ReservedInstances.ReservedInstancesId));
            sut.InstanceCount.Should().Be(100);
            sut.InstanceTenancy.Should().Be(nameof(ReservedInstances.InstanceTenancy));
            sut.InstanceType.Should().Be(nameof(ReservedInstances.InstanceType));
            sut.OfferingClass.Should().Be(nameof(ReservedInstances.OfferingClass));
            sut.State.Should().Be(nameof(ReservedInstances.State));
            sut.UsagePrice.Should().Be(100);
        }

        private AwsAccount GetAccount() => new(new AwsAccount.AwsAccountIdentifier("000000000000", "Test"));

        private IResourceExplorer<AwsAccount, AwsAccount.AwsAccountIdentifier> GetTarget(out Mock<IAmazonEC2> client)
        {
            client = new Mock<IAmazonEC2>();
            var ec2Client = client.Object;
            return new AwsReservedInstanceResourceExplorer(_ => ec2Client);
        }
    }
}
