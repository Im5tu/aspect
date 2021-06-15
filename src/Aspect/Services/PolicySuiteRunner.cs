using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Spectre.Console;

namespace Aspect.Services
{
    internal sealed class PolicySuiteRunner : IPolicySuiteRunner
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicyLoader _policyLoader;
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;

        public PolicySuiteRunner(IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicyCompiler policyCompiler,
            IPolicyLoader policyLoader,
            IBuiltInPolicyProvider builtInPolicyProvider)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
            _policyLoader = policyLoader;
            _builtInPolicyProvider = builtInPolicyProvider;
        }

        public async Task<PolicySuiteRunResult> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default)
        {
            var scopes = suite.Policies?.ToList();
            if (scopes is null || scopes.Count == 0)
                return new PolicySuiteRunResult
                {
                    Errors = new List<string> { "Policy does not specify any policies." },
                    FailedResources = Enumerable.Empty<PolicySuiteRunResult.FailedResource>()
                };

            var failedResources = new ConcurrentDictionary<IResource, List<string>>();
            var errors = new List<string>();
            var result = new PolicySuiteRunResult
            {
                Errors = errors
            };

            var evaluationTask = scopes.Select(scope => RunPolicy(scope, cancellationToken)).ToArray();
            foreach (var task in evaluationTask)
            {
                try
                {
                    var tsk = await task;
                    foreach (var r in tsk.FailedResources)
                        failedResources.GetOrAdd(r.Key, _ => new()).AddRange(r.Value);
                }
                catch (Exception e)
                {
                    errors.Add(e.ToString());
                    AnsiConsole.WriteException(e);
                }
            }

            result.FailedResources = failedResources.Select(x => new PolicySuiteRunResult.FailedResource
            {
                Resource = x.Key,
                FailedPolicies = x.Value
            }).ToList();

            return result;
        }

        private async Task<EvaluationResult> RunPolicy(PolicyElement scope, CancellationToken cancellationToken)
        {
            if (!_cloudProviders.TryGetValue(scope.Type ?? string.Empty, out var provider))
                throw new InvalidOperationException($"Invalid cloud provider: '{scope.Type ?? string.Empty}'");

            // Get all the policies we need
            var policies = GetPolicies(scope);

            // Compile all the policies
            var (evaluators, types) = GetCompiledPolicies(policies);
            var evaluationPipeline = new EvaluationPipeline(evaluators);

            // Load all of the resources
            await LoadResourcesAsync(scope.Regions ?? provider.GetAllRegions(), types, provider, evaluationPipeline, cancellationToken);

            return await evaluationPipeline.CompleteAsync();
        }

        private async Task LoadResourcesAsync(IEnumerable<string> regions, List<Type> types, ICloudProvider provider, EvaluationPipeline evaluationPipeline, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<IResource>>>();
            foreach (var region in regions)
            {
                foreach (var type in types)
                {
                    tasks.Add(provider.DiscoverResourcesAsync(region, type, _ => { }, cancellationToken));
                }
            }

            foreach (var tsk in tasks)
                await evaluationPipeline.AddResources(await tsk);
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
            else if (source.StartsWith("Builtin", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var unit in _builtInPolicyProvider.GetAllResources().Where(x => x.Name.StartsWith(source, StringComparison.OrdinalIgnoreCase) && x.Name.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase)))
                    yield return unit;
            }
            else if (File.GetAttributes(source).HasFlag(FileAttributes.Directory))
            {
                foreach (var unit in _policyLoader.LoadPoliciesInDirectory(source, SearchOption.AllDirectories))
                    yield return unit;
            }
        }
    }
}
