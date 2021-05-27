using System;
using System.Diagnostics;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Represents an account identifier, which may be made up of one or more components
    /// </summary>
    [DebuggerDisplay("{GetAccountIdentifierString(),nq}")]
    public abstract class AccountIdentifier
    {
        /// <summary>
        ///     Get a friendly representation of the account identifier
        /// </summary>
        protected abstract string GetAccountIdentifierString();

        /// <inheritdoc />
        public sealed override string ToString() => GetAccountIdentifierString();

        /// <inheritdoc />
        public sealed override int GetHashCode() => ToString().GetHashCode(StringComparison.Ordinal);
    }
}
