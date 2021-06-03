using System.Diagnostics.CodeAnalysis;
using Aspect.Policies.Suite;

namespace Aspect.Policies.BuiltIn
{
    internal interface IBuiltInPolicyProvider
    {
        bool TryGetPolicy(string name, [NotNullWhen(true)] out BuiltInResourceCompilationUnit? compilationUnit);
        bool TryGetPolicySuite(string name, [NotNullWhen(true)] out PolicySuite? policySuite);
    }
}