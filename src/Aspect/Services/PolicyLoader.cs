using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Spectre.Console;

namespace Aspect.Services
{
    internal class PolicyLoader : IPolicyLoader
    {
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public PolicyLoader(IBuiltInPolicyProvider builtInPolicyProvider, IPolicySuiteSerializer policySuiteSerializer)
        {
            _builtInPolicyProvider = builtInPolicyProvider;
            _policySuiteSerializer = policySuiteSerializer;
        }

        public ValidationResult? ValidateExists(string? path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return ValidationResult.Error("No path has been specified.");

                if (path.StartsWith("builtin\\", StringComparison.OrdinalIgnoreCase))
                {
                    if (_builtInPolicyProvider.GetAllResources().All(x => x.Name != path))
                        return ValidationResult.Error($"The path '{path}' is not a built in resource.");
                }
                else
                {
                    var isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);

                    if (isDirectory)
                    {
                        if (!Directory.Exists(path))
                        {
                            return ValidationResult.Error($"Specified directory '{path}' does not exist.");
                        }
                    }
                    else
                    {
                        if (!File.Exists(path))
                            return ValidationResult.Error($"Specified file '{path}' does not exist.");

                        var fi = new FileInfo(path);

                        if (!fi.Extension.Equals(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase) && !fi.Extension.Equals(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                            return ValidationResult.Error($"Filename must end with either '{FileExtensions.PolicyFileExtension}' or '{FileExtensions.PolicySuiteExtension}'.");
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return ValidationResult.Error(e.Message);
            }
        }

        public PolicySuite? LoadPolicySuite(string path)
        {
            if (!path.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                return null;

            if (path.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
            {
                if (_builtInPolicyProvider.TryGetPolicySuite(path, out var policySuite))
                    return policySuite;
            }
            else
                return _policySuiteSerializer.Deserialize(File.ReadAllText(path));

            return null;
        }

        public CompilationUnit? LoadPolicy(string path)
        {
            if (!path.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                return null;

            if (_builtInPolicyProvider.TryGetPolicy(path, out var unit))
                return unit;

            if (File.Exists(path))
                return new FileCompilationUnit(path);

            return null;
        }

        public IEnumerable<CompilationUnit> LoadPoliciesInDirectory(string path, SearchOption searchOption)
        {
            foreach (var file in Directory.EnumerateFiles(path, $"*{FileExtensions.PolicyFileExtension}", searchOption))
                yield return new FileCompilationUnit(file);
        }
    }
}
