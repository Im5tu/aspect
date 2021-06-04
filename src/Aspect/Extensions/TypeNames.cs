using System;
using System.Linq;

namespace Aspect.Extensions
{
    internal static class TypeNames
    {
        public static string GetFriendlyName(this Type type)
        {
            if (type.IsGenericType)
                return $"{type.Name.Remove(type.Name.IndexOf('`', StringComparison.Ordinal))}<{string.Join(", ", type.GetGenericArguments().Select(GetFriendlyName))}>";

            return type.Name;
        }
    }
}