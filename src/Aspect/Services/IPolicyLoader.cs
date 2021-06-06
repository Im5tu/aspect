using System.Collections.Generic;
using System.IO;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Spectre.Console;

namespace Aspect.Services
{
    internal interface IPolicyLoader
    {
        ValidationResult? ValidateExists(string? path);

        PolicySuite? LoadPolicySuite(string path);
        CompilationUnit? LoadPolicy(string path);
        IEnumerable<CompilationUnit> LoadPoliciesInDirectory(string path, SearchOption searchOption);
    }
}