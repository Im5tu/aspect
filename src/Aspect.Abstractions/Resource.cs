using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     Represents a resource in a cloud provider
    /// </summary>
    public abstract class Resource<TAccount, TAccountIdentifier> : IResource<TAccount, TAccountIdentifier>, IFormatProperties
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
        public string Region { get; }
        /// <inheritdoc />
        public IReadOnlyList<KeyValuePair<string, string>> Tags { get; }

        /// <param name="account">The account that the resource is located in</param>
        /// <param name="name">The name of the resource</param>
        /// <param name="tags">The tags associated with the resource</param>
        /// <param name="type">The type of the resource to be referenced in policies</param>
        /// <param name="region">The region that the resource is located in</param>
        protected Resource(TAccount account, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type, string region)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));
            Type = type ?? throw new ArgumentNullException(nameof(tags));
            Region = region ?? throw new ArgumentNullException(nameof(tags));
        }

        public string Format(string propertyName) => FormatProperty(propertyName);

        protected virtual string FormatProperty(string propertyName)
        {
            return propertyName switch
            {
                nameof(Account) => Account.Id.ToString(),
                nameof(Name) => Name,
                nameof(Type) => Type,
                nameof(Region) => Region,
                nameof(Tags) => string.Join(Environment.NewLine, Tags.Select(x => $"- {x.Key}: {x.Value}")),
                _ => string.Empty
            };
        }
    }
}
