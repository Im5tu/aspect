using Aspect.Abstractions;

namespace Aspect.Providers.Azure.Models
{
    public sealed class AzureAccount : Account<AzureAccount.AzureAccountIdentifier>
    {
        public AzureAccount(AzureAccountIdentifier accountIdentifier)
            : base(accountIdentifier, "AWS")
        {
        }

        public sealed class AzureAccountIdentifier : AccountIdentifier
        {
            protected override string GetAccountIdentifierString() => string.Empty;
        }
    }
}
