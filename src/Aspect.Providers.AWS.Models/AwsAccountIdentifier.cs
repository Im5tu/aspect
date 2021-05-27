using System;
using System.Text.RegularExpressions;
using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    /// <summary>
    ///     Represents an AWS account identifier
    /// </summary>
    public sealed class AwsAccountIdentifier : AccountIdentifier
    {
        /// <summary>
        ///     The account id for the AWS account
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     The friendly name of the AWS account
        /// </summary>
        public string? Name { get; }
        
        /// <param name="id">The account id for the AWS account</param>
        /// <param name="name">The friendly name of the AWS account</param>
        public AwsAccountIdentifier(string id, string name)
            : this (id)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <param name="id">The account id for the AWS account</param>
        public AwsAccountIdentifier(string id)
        {
            if (!Regex.IsMatch(id, "[0-9]{12}"))
                throw new ArgumentException("The parameter {nameof(id)} must be exactly 12 digits. See: https://docs.aws.amazon.com/sdkfornet/v3/apidocs/items/Organizations/TAccount.html");

            Id = id;
        }

        /// <inheritdoc />
        protected override string GetAccountIdentifierString()
        {
            if (Name is null)
                return Id;

            return $"{Id} ({Name})";
        }
    }
}
