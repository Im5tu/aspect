using System;
using System.Threading;
using System.Threading.Tasks;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models;

namespace Aspect.Providers.AWS
{
    /// <summary>
    ///     AWS Account Identity provider
    /// </summary>
    public sealed class AWSAccountIdentityProvider : IAccountIdentityProvider<AwsAccount, AwsAccountIdentifier>
    {
        private readonly IAmazonSecurityTokenService _stsClient;
        private readonly IAmazonIdentityManagementService _iamClient;

        /// <summary></summary>
        public AWSAccountIdentityProvider()
            : this (new AmazonSecurityTokenServiceClient(), new AmazonIdentityManagementServiceClient())
        {
            _stsClient = new AmazonSecurityTokenServiceClient();
            _iamClient = new AmazonIdentityManagementServiceClient();
        }
        /// <summary></summary>
        public AWSAccountIdentityProvider(IAmazonSecurityTokenService stsClient, IAmazonIdentityManagementService iamClient)
        {
            _stsClient = stsClient ?? throw new ArgumentNullException(nameof(stsClient));
            _iamClient = iamClient ?? throw new ArgumentNullException(nameof(iamClient));
        }

        /// <inheritdoc />
        public async Task<AwsAccount> GetAccountAsync(CancellationToken cancellationToken)
        {
            var stsResponse = await _stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest(), cancellationToken);
            //Console.WriteLine("STS Response:");
            //Console.WriteLine(JsonSerializer.Serialize(stsResponse));

            var request = new ListAccountAliasesRequest();
            var response = await _iamClient.ListAccountAliasesAsync(request, cancellationToken);

            //Console.WriteLine("Account Aliases:");
            //Console.WriteLine(string.Join(",", response.AccountAliases));
            //Console.WriteLine();

            if (response.AccountAliases.Count == 0)
                return new AwsAccount(new AwsAccountIdentifier(stsResponse.Account));

            return new AwsAccount(new AwsAccountIdentifier(stsResponse.Account, response.AccountAliases[0]));
        }
    }
}
