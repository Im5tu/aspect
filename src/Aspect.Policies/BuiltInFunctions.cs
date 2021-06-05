using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aspect.Policies
{
    internal static class BuiltInFunctions
    {
        private static readonly Dictionary<string, MethodInfo> _functions;

        static BuiltInFunctions()
        {
            _functions = typeof(BuiltInFunctions).GetTypeInfo().DeclaredMethods
                .Where(x => x.Name != nameof(TryLookupBuiltInFunction))
                .ToDictionary(x => x.Name.LowerFirstLetter() + x.GetParameters().Length, x => x);
        }

        internal static bool TryLookupBuiltInFunction(string name, out MethodInfo? method) => _functions.TryGetValue(name, out method);

        internal static bool Contains(string input, string value)
            => Contains(input, value, false);
        internal static bool Contains(string input, string value, bool caseSensitive)
        {
            if (input is null)
                return false;

            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return input.Contains(value, comparer);
        }

        internal static bool StartsWith(string input, string value)
            => StartsWith(input, value, false);
        internal static bool StartsWith(string input, string value, bool caseSensitive)
        {
            if (input is null)
                return false;

            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return input.StartsWith(value, comparer);
        }

        internal static bool EndsWith(string input, string value)
            => EndsWith(input, value, false);
        internal static bool EndsWith(string input, string value, bool caseSensitive)
        {
            if (input is null)
                return false;

            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            return input.EndsWith(value, comparer);
        }

        internal static bool Matches(IEnumerable<KeyValuePair<string, string>> input, string key, string value)
            => Matches(input, key, value, false);
        internal static bool Matches(IEnumerable<KeyValuePair<string, string>> input, string key, string value, bool caseSensitive)
        {
            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach (var el in input)
            {
                if (string.Equals(key, el.Key, comparer) && string.Equals(value, el.Value, comparer))
                    return true;
            }

            return false;
        }

        internal static bool HasKey(IEnumerable<KeyValuePair<string, string>> input, string key)
            => HasKey(input, key, false);
        internal static bool HasKey(IEnumerable<KeyValuePair<string, string>> input, string key, bool caseSensitive)
        {
            var comparer = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            foreach (var el in input)
            {
                if (string.Equals(key, el.Key, comparer))
                    return true;
            }

            return false;
        }
    }
}
