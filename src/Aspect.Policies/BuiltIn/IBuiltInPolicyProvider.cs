using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;

namespace Aspect.Policies.BuiltIn
{
    internal interface IBuiltInPolicyProvider
    {
        IEnumerable<BuiltInResourceCompilationUnit> GetAllResources();
        bool TryGetPolicy(string name, [NotNullWhen(true)] out BuiltInResourceCompilationUnit? compilationUnit);
        bool TryGetPolicySuite(string name, [NotNullWhen(true)] out PolicySuite? policySuite);
    }
}
