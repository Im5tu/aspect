using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Aspect.Abstractions
{
    public abstract class Resource<TAccount, TAccountIdentifier> : IResource<TAccount, TAccountIdentifier>, IFormatProperties
        where TAccount : Account<TAccountIdentifier>
        where TAccountIdentifier : AccountIdentifier
    {
        [Description("The account that is associated with the resource.")]
        public TAccount Account { get; }
        [Description("The name of the resource.")]
        public string Name { get; }
        [Description("The type of the resource. This is what is used in policy evaluation.")]
        public string Type { get; }
        [Description("The region in which this resource is located.")]
        public string Region { get; }
        [Description("The tags that are associated with the resource. Each entry has two properties: Key/Value.")]
        public IReadOnlyList<KeyValuePair<string, string>> Tags { get; }

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
                _ => this.GetType().GetProperty(propertyName)!.GetMethod!.Invoke(this, Array.Empty<object>())?.ToString() ?? string.Empty
            };
        }
    }
}
