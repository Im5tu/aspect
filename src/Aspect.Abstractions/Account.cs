using System;
using System.Diagnostics;

namespace Aspect.Abstractions
{
    /// <summary>
    ///     The basis for all cloud provider accounts
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public abstract class Account<T> : IFormatProperties
        where T : AccountIdentifier
    {
        /// <summary>
        ///     The account identifier
        /// </summary>
        public T Id { get; }
        /// <summary>
        ///     The type of the account, eg: AWS/Azure/GCP
        /// </summary>
        public string Type { get; }

        /// <param name="id">The account identifier for the specified account</param>
        /// <param name="type">The type of the account, eg: AWS/Azure/GCP</param>
        protected Account(T id, string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));

            Id = id ?? throw new ArgumentNullException(nameof(id));
            Type = type;
        }

        /// <inheritdoc />
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
