using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aspect.Abstractions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetResourceTypes(this Assembly assembly)
            => assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsAssignableTo(typeof(IResource)));

        public static string ValueOrDefault(this string? str, string defaultValue)
            => string.IsNullOrWhiteSpace(str) ? defaultValue : str.Trim();

        public static string ValueOrEmpty(this string? str)
            => str?.Trim() ?? string.Empty;

        public static IEnumerable<T> ValueOrEmpty<T>(this IEnumerable<T>? collection)
            => collection?.ToList() ?? Enumerable.Empty<T>();
    }
}
