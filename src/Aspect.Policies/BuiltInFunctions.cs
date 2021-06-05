using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Aspect.Policies
{
    internal static class BuiltInFunctions
    {
        private static readonly Dictionary<string, MethodInfo> _functions;

        static BuiltInFunctions()
        {
            var notAccessibleFunctions = new[] {nameof(TryLookupBuiltInFunction), nameof(ConvertToFriendlyMethodName)};
            _functions = typeof(BuiltInFunctions).GetTypeInfo().DeclaredMethods
                .Where(x => !notAccessibleFunctions.Contains(x.Name))
                .ToDictionary(x => ConvertToFriendlyMethodName(x.Name, x.GetParameters().Select(x => x.ParameterType)), x => x);
        }

        internal static bool TryLookupBuiltInFunction(string name, [NotNullWhen(true)] out MethodInfo? method) => _functions.TryGetValue(name, out method);
        internal static bool TryLookupBuiltInFunction(string name, IEnumerable<Type> parameters, [NotNullWhen(true)] out MethodInfo? method) => _functions.TryGetValue(ConvertToFriendlyMethodName(name, parameters), out method);

        internal static string ConvertToFriendlyMethodName(string function, IEnumerable<Type> parameters)
        {
            var names = new List<string?>();

            foreach (var param in parameters)
            {
                if (param.IsAssignableTo(typeof(string)))
                    names.Add("string");
                else if (param.IsAssignableTo(typeof(IEnumerable)))
                    names.Add("collection");
                else if (param.IsAssignableTo(typeof(Int16)) || param.IsAssignableTo(typeof(Int32)) || param.IsAssignableTo(typeof(Int64)) || param.IsAssignableTo(typeof(Decimal)))
                    names.Add("number");
                else
                    names.Add(param.Name);
            }

            return $"{function.LowerFirstLetter()}[{string.Join(",", names)}]";
        }

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

        internal static bool Contains(IEnumerable<KeyValuePair<string, string>> input, string key, string value)
            => Contains(input, key, value, false);
        internal static bool Contains(IEnumerable<KeyValuePair<string, string>> input, string key, string value, bool caseSensitive)
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

        internal static bool Matches(string input, string regex) => new Regex(regex).IsMatch(input);
    }
}
