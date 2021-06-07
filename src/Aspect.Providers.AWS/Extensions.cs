using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.EC2.Model;

namespace Aspect.Providers.AWS
{
    internal static class Extensions
    {
        internal static string GetName(this List<Tag> tags)
            => tags.FirstOrDefault(x => "name".Equals(x.Key, StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;

        internal static IReadOnlyList<KeyValuePair<string, string>> Convert(this List<Tag> tags)
            => tags.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)).ToList();
    }
}
