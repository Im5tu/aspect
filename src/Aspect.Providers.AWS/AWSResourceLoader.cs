using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.SecurityToken;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;
using Aspect.Providers.AWS.Resources.EC2;

namespace Aspect.Providers.AWS
{
    /// <summary>
    ///
    /// </summary>
    public class AWSResourceLoader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IResource>> LoadResourcesForType(string type, Action<string> statusUpdater)
        {
            statusUpdater("Authenticating account");
            var account = await GetCurrentAwsAccountAsync();

            if (nameof(AwsSecurityGroup).Equals(type, StringComparison.OrdinalIgnoreCase))
            {
                statusUpdater("Loading security groups");
                // TODO :: Pull from variable or prompt
                return (await new SecurityGroupResourceExplorer().DiscoverResourcesAsync(account, "eu-west-1", default)).ToList();
            }

            return new IResource[0];
        }

        private static async Task<AwsAccount> GetCurrentAwsAccountAsync()
        {
            var accountProvider = new AWSAccountIdentityProvider(new AmazonSecurityTokenServiceClient(), new AmazonIdentityManagementServiceClient());
            return (AwsAccount) await accountProvider.GetAccountAsync(default);
        }
    }
}
