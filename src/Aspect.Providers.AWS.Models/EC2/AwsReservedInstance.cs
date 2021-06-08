using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Aspect.Providers.AWS.Models.EC2
{
    [Description("Reserved Instances (RI) provide a significant discount (up to 72%) compared to On-Demand pricing for EC2 Instances and provide a capacity reservation when used in a specific Availability Zone.")]
    public class AwsReservedInstance : AwsResource
    {
        [Description("The Availability Zone in which the Reserved Instance can be used.")]
        public string? AvailabilityZone { get; init; }

        [Description("The currency of the Reserved Instance. It's specified using ISO 4217 standard currency codes.")]
        public string? CurrencyCode { get; init; }

        [Description("The time when the Reserved Instance expires.")]
        public DateTime? Expires { get; init; }

        [Description("The purchase price of the Reserved Instance.")]
        public float FixedPrice { get; init; }

        [Description("The ID of the Reserved Instance.")]
        public string? Id { get; init; }

        [Description("The number of reservations purchased.")]
        public int InstanceCount { get; init; }

        [Description("The tenancy of the instance.")]
        public string? InstanceTenancy { get; init; }

        [Description("The instance type on which the Reserved Instance can be used.")]
        public string? InstanceType { get; init; }

        [Description("The offering class of the Reserved Instance.")]
        public string? OfferingClass { get; init; }

        [Description("The state of the Reserved Instance purchase.")]
        public string? State { get; init; }

        [Description("The usage price of the Reserved Instance, per hour.")]
        public float UsagePrice { get; init; }

        public AwsReservedInstance(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string region)
            : base(account, arn, name, tags, nameof(AwsReservedInstance), region)
        {
        }
    }
}
