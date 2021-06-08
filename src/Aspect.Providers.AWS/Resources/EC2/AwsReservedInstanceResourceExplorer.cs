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
    internal sealed class AwsReservedInstanceResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsReservedInstanceResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsReservedInstance))
        {
            _creator = creator;
        }

        public AwsReservedInstanceResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            var response = await ec2Client.DescribeReservedInstancesAsync(new DescribeReservedInstancesRequest(), cancellationToken);
            foreach (var ri in response.ReservedInstances)
            {
                var arn = GenerateArn(account, region, "ec2", $"reserved-instances/{ri.ReservedInstancesId}");
                result.Add(new AwsReservedInstance(account, arn, ri.Tags.GetName(), ri.Tags.Convert(), region.SystemName)
                {
                    AvailabilityZone = ri.AvailabilityZone.ValueOrEmpty(),
                    CurrencyCode = ri.CurrencyCode?.Value.ValueOrEmpty(),
                    Expires = ri.End,
                    FixedPrice = ri.FixedPrice,
                    Id = ri.ReservedInstancesId.ValueOrEmpty(),
                    InstanceCount = ri.InstanceCount,
                    InstanceTenancy = ri.InstanceTenancy?.Value.ValueOrEmpty(),
                    InstanceType = ri.InstanceType?.Value.ValueOrEmpty(),
                    OfferingClass = ri.OfferingClass?.Value.ValueOrEmpty(),
                    State = ri.State?.Value.ValueOrEmpty(),
                    UsagePrice = ri.UsagePrice,
                });
            }

            return result;
        }
    }
}
