using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Aspect.Policies.Suite;

namespace Aspect.Policies.BuiltIn
{
    internal sealed class BuiltInPolicyProvider : IBuiltInPolicyProvider
    {
        private readonly IPolicySuiteSerializer _policySuiteSerializer;
        private readonly Dictionary<string, BuiltInResourceCompilationUnit> _resources;

        public BuiltInPolicyProvider(IPolicySuiteSerializer policySuiteSerializer)
        {
            _policySuiteSerializer = policySuiteSerializer;
            var assembly = typeof(BuiltInPolicyProvider).Assembly;
            _resources = assembly.GetManifestResourceNames()
                .Select(x => (key: x.Remove(0, "Aspect.Policies.".Length)
                        .Replace("BuiltIn.AWS.", "BuiltIn\\AWS\\", StringComparison.OrdinalIgnoreCase)
                        .Replace("BuiltIn.Azure.", "BuiltIn\\Azure\\", StringComparison.OrdinalIgnoreCase),
                    lookup: x)
                )
                .ToDictionary(x => x.key, x =>
                {
                    // manifest stream cannot be null here, but the compiler doesn't know that
                    using var sr = new StreamReader(assembly.GetManifestResourceStream(x.lookup)!);
                    return new BuiltInResourceCompilationUnit(x.key, sr.ReadToEnd());
                }, StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<BuiltInResourceCompilationUnit> GetAllResources() => _resources.Values;

        public bool TryGetPolicy(string name, [NotNullWhen(true)] out BuiltInResourceCompilationUnit? compilationUnit)
            => _resources.TryGetValue(name, out compilationUnit);

        public bool TryGetPolicySuite(string name, [NotNullWhen(true)] out PolicySuite? policySuite)
        {
            policySuite = null;
            if (!_resources.TryGetValue(name, out var compilationUnit))
                return false;

            policySuite = _policySuiteSerializer.Deserialize(compilationUnit.GetAllText());
            return true;
        }
    }
}
