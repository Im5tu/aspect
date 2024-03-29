﻿using System;
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
    internal sealed class AWSAccountIdentityProvider : IAccountIdentityProvider<AwsAccount, AwsAccount.AwsAccountIdentifier>
    {
        private readonly IAmazonSecurityTokenService _stsClient;
        private readonly IAmazonIdentityManagementService _iamClient;
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1);
        private AwsAccount? _account;

        public AWSAccountIdentityProvider()
            : this (new AmazonSecurityTokenServiceClient(), new AmazonIdentityManagementServiceClient())
        {
        }

        public AWSAccountIdentityProvider(IAmazonSecurityTokenService stsClient, IAmazonIdentityManagementService iamClient)
        {
            _stsClient = stsClient ?? throw new ArgumentNullException(nameof(stsClient));
            _iamClient = iamClient ?? throw new ArgumentNullException(nameof(iamClient));
        }

        public async Task<AwsAccount> GetAccountAsync(CancellationToken cancellationToken)
        {
            if (_account is { })
                return _account;

            try
            {
                await _asyncLock.WaitAsync(cancellationToken);

                if (_account is { })
                    return _account;

                var stsResponse = await _stsClient.GetCallerIdentityAsync(new GetCallerIdentityRequest(), cancellationToken);
                var request = new ListAccountAliasesRequest();
                var response = await _iamClient.ListAccountAliasesAsync(request, cancellationToken);
                if (response.AccountAliases.Count == 0)
                    return new AwsAccount(new AwsAccount.AwsAccountIdentifier(stsResponse.Account));

                return _account ??= new AwsAccount(new AwsAccount.AwsAccountIdentifier(stsResponse.Account, response.AccountAliases[0]));
            }
            finally
            {
                _asyncLock.Release();
            }
        }
    }
}
