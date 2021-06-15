using System.Collections.Generic;
using System.Diagnostics;
using Aspect.Abstractions;

namespace Aspect.Providers.Azure.Models
{
    public abstract class AzureResource : Resource<AzureAccount, AzureAccount.AzureAccountIdentifier>
    {
        /// <inheritDoc />
        protected AzureResource(AzureAccount account, string name, string cloudId, IReadOnlyList<KeyValuePair<string, string>> tags, string type, string region)
            : base(account, name, cloudId, tags, type, region)
        {
        }
    }
}
