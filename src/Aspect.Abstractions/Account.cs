using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Aspect.Abstractions
{
    [DebuggerDisplay("{ToString(),nq}")]
    public abstract class Account<T> : IFormatProperties
        where T : AccountIdentifier
    {
        [Description("The account identifier specific to the cloud provider")]
        public T Id { get; }

        [Description("The type of the account, eg: AWS/Azure/GCP")]
        public string Type { get; }

        protected Account(T id, string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));

            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type;
        }

        public override string ToString() => $"{Type} Account: {Id}";

        public virtual string Format(string propertyName)
        {
            return propertyName switch
            {
                nameof(Id) => Id.ToString(),
                nameof(Type) => Type,
                _ => string.Empty
            };
        }
    }
}
