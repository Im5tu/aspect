using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Aspect.Abstractions;

namespace Aspect.Providers.AWS.Models
{
    public abstract class AwsResource : Resource<AwsAccount, AwsAccount.AwsAccountIdentifier>
    {
        protected AwsResource(AwsAccount account, string arn, string name, IReadOnlyList<KeyValuePair<string, string>> tags, string type, string region)
            : base(account, name, arn, tags, type, region)
        {
        }
    }
}
