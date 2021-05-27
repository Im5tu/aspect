using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    /// <summary>
    ///     Represents an AWS Resource
    /// </summary>
    [DebuggerDisplay("{Arn,nq}")]
    public abstract class AwsResource : Resource<AwsAccount, AwsAccountIdentifier>
    {
        /// <summary>
        ///     The AWS ARN identifying this resource
        /// </summary>
        public string Arn { get; }

        /// <inheritDoc />
        protected AwsResource(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type)
            : base(account, name, tags, type)
        {
            if (string.IsNullOrWhiteSpace(arn))
                throw new ArgumentNullException(nameof(arn), "ARN cannot be null, empty or whitespace.");

            Arn = arn;
        }
    }
}
