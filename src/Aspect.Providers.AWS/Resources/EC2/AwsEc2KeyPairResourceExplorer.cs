using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsEc2KeyPairResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsEc2KeyPairResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsEc2KeyPair))
        {
            _creator = creator;
        }

        public AwsEc2KeyPairResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<IResource>();

            var response = await ec2Client.DescribeKeyPairsAsync(cancellationToken);

            foreach (var keyPair in response.KeyPairs)
            {
                var arn = GenerateArn(account, region, "ec2", $"key-pair/{keyPair.KeyPairId}");
                result.Add(new AwsEc2KeyPair(account, arn, keyPair.KeyName, keyPair.Tags.Convert(), region.SystemName)
                {
                    Id = keyPair.KeyPairId.ValueOrEmpty(),
                    Fingerprint = keyPair.KeyFingerprint.ValueOrEmpty()
                });
            }

            return result;
        }
    }
}
