using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aspect.Abstractions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetResourceTypes(this Assembly assembly)
        {
            return assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsAssignableTo(typeof(IResource)));
        }
    }
}
