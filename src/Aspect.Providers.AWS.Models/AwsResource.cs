using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    [DebuggerDisplay("{Arn,nq}")]
    public abstract class AwsResource : Resource<AwsAccount, AwsAccount.AwsAccountIdentifier>
    {
        [Description("The Amazon Resource Names (ARN) uniquely identifying an AWS resource.")]
        public string Arn { get; }

        protected AwsResource(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type, string region)
            : base(account, name, tags, type, region)
        {
            if (string.IsNullOrWhiteSpace(arn))
                throw new ArgumentNullException(nameof(arn), "ARN cannot be null, empty or whitespace.");

            Arn = arn;
        }

        protected override string FormatProperty(string propertyName)
        {
            return propertyName switch
            {
                nameof(Arn) => Arn,
                _ => base.FormatProperty(propertyName)
            };
        }
    }
}
