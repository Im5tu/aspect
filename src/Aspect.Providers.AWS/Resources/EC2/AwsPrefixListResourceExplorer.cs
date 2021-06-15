using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Providers.AWS.Resources.EC2
{
    internal sealed class AwsPrefixListResourceExplorer : AWSResourceExplorer
    {
        private readonly Func<AmazonEC2Config, IAmazonEC2> _creator;

        public AwsPrefixListResourceExplorer(Func<AmazonEC2Config, IAmazonEC2> creator)
            : base(typeof(AwsPrefixList))
        {
            _creator = creator;
        }

        public AwsPrefixListResourceExplorer()
            : this(config => new AmazonEC2Client(config))
        {
        }

        protected override async Task<IEnumerable<IResource>> DiscoverResourcesAsync(AwsAccount account, RegionEndpoint region, CancellationToken cancellationToken)
        {
            if (region == RegionEndpoint.APNortheast3)
                return Enumerable.Empty<IResource>();

            using var ec2Client = _creator(new AmazonEC2Config { RegionEndpoint = region });
            var result = new List<AwsPrefixList>();

            var detailTasks = new List<Task>();

            string? nextToken = null;
            do
            {
                var response = await ec2Client.DescribeManagedPrefixListsAsync(new DescribeManagedPrefixListsRequest { NextToken = nextToken, Filters = new List<Filter>
                {
                    new() { Name = "owner-id", Values = new() { account.Id.Id } }
                }}, cancellationToken);
                nextToken = response.NextToken;

                foreach (var pl in response.PrefixLists)
                {
                    var list = new AwsPrefixList(account, pl.PrefixListArn, pl.PrefixListName.ValueOrDefault("Unknown"), pl.Tags.Convert(), region.SystemName)
                    {
                        AddressFamily = pl.AddressFamily.ValueOrEmpty(),
                        Id = pl.PrefixListId.ValueOrEmpty(),
                        OwnerId = pl.OwnerId.ValueOrEmpty(),
                        State = pl.State?.Value.ValueOrEmpty(),
                    };
                    result.Add(list);

                    detailTasks.Add(GetDetailsForResource(list, ec2Client));
                }
            } while (!string.IsNullOrWhiteSpace(nextToken));

            await Task.WhenAll(detailTasks);

            return result;
        }

        private async Task GetDetailsForResource(AwsPrefixList list, IAmazonEC2 ec2Client)
        {
            var cidrs = new List<string>();
            list.Cidrs = cidrs;
            string? nextToken = null;
            do
            {
                var response = await ec2Client.GetManagedPrefixListEntriesAsync(new GetManagedPrefixListEntriesRequest {PrefixListId = list.Id});
                nextToken = response?.NextToken;

                cidrs.AddRange(response?.Entries?.Select(x => x.Cidr.ValueOrEmpty()) ?? Enumerable.Empty<string>());
            } while (!string.IsNullOrWhiteSpace(nextToken));
        }
    }
}
