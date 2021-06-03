using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Policies
{
    internal static class Types
    {
        private static readonly Dictionary<string, Type> _awsTypes = new();
        private static readonly Dictionary<string, Type> _azureTypes = new();

        static Types()
        {
            foreach (var t in typeof(AwsSecurityGroup).Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IResource)) && !x.IsAbstract && x.IsClass))
                AddResource(t);
        }

        internal static void AddResource<T>()
            where T : IResource
            => AddResource(typeof(T));

        internal static void AddResource(Type type)
        {
            if (type.IsAbstract || !type.IsAssignableTo(typeof(IResource)))
                return;

            AddResource(type.Name, type);
        }

        internal static void AddResource(string name, Type type)
        {
            if (type.IsAbstract || !type.IsAssignableTo(typeof(IResource)))
                return;

            _awsTypes[name] = type;
        }

        internal static IEnumerable<Type> GetTypes(string provider)
        {
            if ("AWS".Equals(provider, StringComparison.OrdinalIgnoreCase))
                return _awsTypes.Values;
            if ("Azure".Equals(provider, StringComparison.OrdinalIgnoreCase))
                return _azureTypes.Values;

            return Enumerable.Empty<Type>();
        }

        internal static Type? GetType(string name) => _awsTypes.TryGetValue(name, out var type) ? type : _azureTypes.TryGetValue(name, out type) ? type : null;
        internal static Type? GetType(string name, string provider)
        {
            if ("AWS".Equals(provider, StringComparison.OrdinalIgnoreCase) && _awsTypes.TryGetValue(name, out var type))
                return type;
            if ("Azure".Equals(provider, StringComparison.OrdinalIgnoreCase) && _azureTypes.TryGetValue(name, out type))
                return type;

            return null;
        }
    }
}
