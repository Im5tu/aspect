using System.Threading;
using System.Threading.Tasks;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Account identity provider
    /// </summary>
    public interface IAccountIdentityProvider<TAccount, TAccountIdentifier> where TAccount : Account<TAccountIdentifier> where TAccountIdentifier : AccountIdentifier
    {
        /// <summary>
        ///     Returns the current account in use
        /// </summary>
        Task<TAccount> GetAccountAsync(CancellationToken cancellationToken);
    }
}
