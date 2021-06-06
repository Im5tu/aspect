using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.CompilerServices.Generator;
using Aspect.Policies.Suite;

namespace Aspect.Services
{
    internal sealed class PolicySuiteRunner : IPolicySuiteRunner
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicyLoader _policyLoader;

        public PolicySuiteRunner(IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicyCompiler policyCompiler,
            IPolicyLoader policyLoader)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
            _policyLoader = policyLoader;
        }

        public async Task<IEnumerable<PolicySuiteRunResult>> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default)
        {
            var scopes = suite.Policies?.ToList();
            if (scopes is null || scopes.Count == 0)
                return Enumerable.Empty<PolicySuiteRunResult>();

            var resultScopes = scopes.Select(scope => RunPolicy(scope, cancellationToken)).ToArray();
            return await Task.WhenAll(resultScopes);
        }

        private async Task<PolicySuiteRunResult> RunPolicy(PolicyElement scope, CancellationToken cancellationToken)
        {
            if (!_cloudProviders.TryGetValue(scope.Type ?? string.Empty, out var provider))
            {
                return new PolicySuiteRunResult { Error = $"Invalid cloud provider: '{scope.Type ?? string.Empty}'" };
            }

            // Get all the policies we need
            var policies = GetPolicies(scope);

            // Compile all the policies
            var (evaluators, types) = GetCompiledPolicies(policies);

            // Load all of the resources
            var resources = await LoadResourcesAsync(scope.Regions!, types, provider, cancellationToken);

            // Validation
            var failedResources = new List<PolicySuiteRunResult.FailedResource>();
            foreach (var resource in resources)
            {
                foreach (var eval in evaluators)
                {
                    if (eval.Evaluator!(resource) == ResourcePolicyExecution.Failed)
                        failedResources.Add(new PolicySuiteRunResult.FailedResource { Resource = resource, Source = eval.Source.GetPolicyName() });
                }
            }

            return new PolicySuiteRunResult { FailedResources = failedResources };
        }

        private async Task<List<IResource>> LoadResourcesAsync(IEnumerable<string> regions, List<Type> types, ICloudProvider provider, CancellationToken cancellationToken)
        {
            var resources = new List<IResource>();
            foreach (var region in regions)
            {
                foreach (var type in types)
                {
                    resources.AddRange(await provider.DiscoverResourcesAsync(region, type, _ => { }, cancellationToken));
                }
            }

            return resources;
        }
        private List<CompilationUnit> GetPolicies(PolicyElement scope)
        {
            var policies = new List<CompilationUnit>();
            foreach (var policy in scope.Policies ?? Enumerable.Empty<string>())
            {
                policies.AddRange(LoadPoliciesByName(policy));
            }

            return policies;
        }
        private (List<ExecutablePolicy> evaluators, List<Type> resourceTypes) GetCompiledPolicies(List<CompilationUnit> policies)
        {
            var compiledPolicies = policies.Select(x =>
            {
                var context = new CompilationContext(x);
                var func = _policyCompiler.GetFunctionForPolicy(context, out var resource);
                if (func is null || resource is null)
                {
                    context.WriteCompilationResultToConsole();
                    return null;
                }

                return new ExecutablePolicy(func, resource, context.Source);
            }).Where(x => x is not null).Select(x => x!).ToList();
            var types = compiledPolicies.Select(x => x!.Resource!).Distinct().ToList();
            return (compiledPolicies, types);
        }
        private IEnumerable<CompilationUnit> LoadPoliciesByName(string source)
        {
            if (source.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                yield break;

            if (source.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
            {
                var result = _policyLoader.LoadPolicy(source);
                if (result is { })
                    yield return result;
            }
            else if (File.GetAttributes(source).HasFlag(FileAttributes.Directory))
            {
                foreach (var unit in _policyLoader.LoadPoliciesInDirectory(source, SearchOption.AllDirectories))
                    yield return unit;
            }
        }

        private class ExecutablePolicy
        {
            internal Func<IResource, ResourcePolicyExecution> Evaluator { get; }
            internal Type Resource { get; }
            internal CompilationUnit Source { get; }

            public ExecutablePolicy(Func<IResource, ResourcePolicyExecution> evaluator, Type resource, CompilationUnit source)
            {
                Evaluator = evaluator;
                Resource = resource;
                Source = source;
            }
        }
    }
}
