using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    [Description("The AWS account resource describes the details of the account that a specified resource belongs to.")]
    public sealed class AwsAccount : Account<AwsAccount.AwsAccountIdentifier>
    {
        public AwsAccount(AwsAccountIdentifier accountIdentifier)
            : base(accountIdentifier, "AWS")
        {
        }

        public sealed class AwsAccountIdentifier : AccountIdentifier
        {
            [Description("The 12 digit numeric identifer of the account.")]
            public string Id { get; }

            [Description("The friendly name of the account if it's available. Usually only supported by organisational accounts.")]
            public string? Name { get; }

            public AwsAccountIdentifier(string id, string name)
                : this (id)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                Name = name;
            }

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
}
