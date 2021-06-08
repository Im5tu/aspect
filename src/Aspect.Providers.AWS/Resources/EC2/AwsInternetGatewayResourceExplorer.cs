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
    internal sealed class AwsInternetGatewayResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsInternetGatewayResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsInternetGateway))
        {
            _creator = creator;
        }

        public AwsInternetGatewayResourceExplorer()
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
                var response = await ec2Client.DescribeInternetGatewaysAsync(new DescribeInternetGatewaysRequest { NextToken = nextToken }, cancellationToken);
                nextToken = response.NextToken;

                foreach (var gw in response.InternetGateways)
                {
                    var arn = GenerateArn(account, region, "ec2", $"internet-gateway/{gw.InternetGatewayId.ValueOrEmpty()}");
                    result.Add(new AwsInternetGateway(account, arn, gw.Tags.GetName(), gw.Tags.Convert(), region.SystemName)
                    {
                       Id = gw.InternetGatewayId.ValueOrEmpty(),
                       OwnerId = gw.OwnerId.ValueOrEmpty(),
                       Attachments = Map(gw.Attachments)
                    });
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            return result;
        }

        private IEnumerable<AwsInternetGateway.Attachment> Map(List<InternetGatewayAttachment>? gwAttachments)
        {
            var result = new List<AwsInternetGateway.Attachment>();

            foreach (var att in gwAttachments ?? Enumerable.Empty<InternetGatewayAttachment>())
            {
                result.Add(new AwsInternetGateway.Attachment
                {
                    State = att.State?.Value.ValueOrEmpty(),
                    VpcId = att.VpcId.ValueOrEmpty()
                });
            }

            return result;
        }
    }
}
