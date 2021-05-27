using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;
using Aspect.Providers.AWS.Models.EC2;

namespace Aspect.Policies
{
    internal static class Types
    {
        private static readonly Dictionary<string, Type> _types = new();

        static Types()
        {
            foreach (var t in typeof(AwsSecurityGroup).Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(IResource)) && !x.IsAbstract))
                AddResource(t);
        }

        internal static void AddResource<T>()
            where T : IResource
            => AddResource(typeof(T));

        internal static void AddResource(Type type)
        {
            if (type.IsAbstract || !type.IsAssignableTo(typeof(IResource)))
                return;

            // TODO :: TASK :: How to manage this? Source Gen?
            AddResource(type.Name, type);
        }

        internal static void AddResource(string name, Type type)
        {
            if (type.IsAbstract || !type.IsAssignableTo(typeof(IResource)))
                return;

            _types[name] = type;
        }

        internal static Type? GetType(string name) => _types.TryGetValue(name, out var type) ? type : null;
    }
}
