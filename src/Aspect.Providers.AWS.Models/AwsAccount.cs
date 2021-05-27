using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    /// <summary>
    ///     Represents an AWS account
    /// </summary>
    public sealed class AwsAccount : Account<AwsAccountIdentifier>
    {
        /// <param name="accountIdentifier">The account identifier to use</param>
        public AwsAccount(AwsAccountIdentifier accountIdentifier)
            : base(accountIdentifier, "AWS")
        {
        }
    }
}
