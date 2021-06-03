using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspect.Abstractions
{
    public interface IResourceTypeLocator
    {
        bool TryLocateType(string resourceType, [NotNullWhen(true)] out Type? type);
    }
}
