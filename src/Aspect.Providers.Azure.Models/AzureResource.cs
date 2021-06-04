using System.Collections.Generic;
using System.Diagnostics;
using Aspect.Abstractions;

namespace Aspect.Providers.Azure.Models
{
    [DebuggerDisplay("{Name,nq}")]
    public abstract class AzureResource : Resource<AzureAccount, AzureAccount.AzureAccountIdentifier>
    {
        /// <inheritDoc />
        protected AzureResource(AzureAccount account, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type, string region)
            : base(account, name, tags, type, region)
        {
        }
    }
}
