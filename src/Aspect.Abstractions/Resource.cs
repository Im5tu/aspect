using System;
using System.Collections.Generic;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Represents a resource in a cloud provider
    /// </summary>
    public abstract class Resource<TAccount, TAccountIdentifier> : IResource<TAccount, TAccountIdentifier>
        where TAccount : Account<TAccountIdentifier>
        where TAccountIdentifier : AccountIdentifier
    {
        /// <inheritdoc />
        public TAccount Account { get; }
        /// <inheritdoc />
        public string Name { get; }
        /// <inheritdoc />
        public string Type { get; }
        /// <inheritdoc />
        public IReadOnlyList<KeyValuePair<string, string>> Tags { get; }

        /// <param name="account">The account that the resource is located in</param>
        /// <param name="name">The name of the resource</param>
        /// <param name="tags">The tags associated with the resource</param>
        /// <param name="type">The type of the resource to be referenced in policies</param>
        protected Resource(TAccount account, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
            Type = type;
        }
    }
}
