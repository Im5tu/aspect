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
using Aspect.Policies.CompilerServices.Generator;
using Aspect.Providers.AWS;
using Aspect.Providers.AWS.Models;

namespace Aspect.Runners
{
    internal sealed class PolicySuiteRunner : IPolicySuiteRunner
    {
        private readonly Dictionary<Type, IResourceExplorer<AwsAccount, AwsAccountIdentifier>> _awsProviders = new();
        private readonly Dictionary<Type, IResourceExplorer> _azureProviders = new();

        public PolicySuiteRunner(IEnumerable<IResourceExplorer> resourceExplorers)
        {
            foreach (var provider in resourceExplorers)
            {
                if (provider is IResourceExplorer<AwsAccount, AwsAccountIdentifier> awsProvider)
                    _awsProviders[awsProvider.ResourceType] = awsProvider;
                // TODO :: Add Azure
            }
        }

        public async Task<IEnumerable<PolicySuiteRunResult>> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default)
        {
            var scopes = suite.Scopes?.ToList();
            if (scopes is null || scopes.Count == 0)
                return Enumerable.Empty<PolicySuiteRunResult>();

            var resultScopes = scopes.Select(scope => RunPolicy(scope, cancellationToken)).ToArray();
            return await Task.WhenAll(resultScopes);
        }

        private async Task<PolicySuiteRunResult> RunPolicy(PolicySuiteScope scope, CancellationToken cancellationToken)
        {
            if (!new[] {"AWS", "Azure"}.Contains(scope.Type ?? string.Empty))
            {
                // TODO :: ERROR : Sort this out
            }

            // Get all the policies we need
            var policies = GetPolicies(scope);

            // Compile all the policies
            var (evaluators, types) = GetCompiledPolicies(policies);

            // Load all of the resources
            List<IResource> resources;
            if (scope.Type!.Equals("AWS", StringComparison.OrdinalIgnoreCase))
            {
                resources = await LoadAWSResourcesAsync(scope.Regions!, types, cancellationToken);
            }
            else
            {
                resources = new List<IResource>();
            }

            // Validation
            var failedResources = new List<(IResource resource, CompilationUnit source)>();
            foreach (var resource in resources)
            {
                foreach (var eval in evaluators)
                {
                    if (eval.Evaluator!(resource) == ResourcePolicyExecution.Failed)
                        failedResources.Add((resource, eval.Source));
                }
            }

            // TODO :: Dump resources properly
            Console.WriteLine("Failed resources:");
            foreach (var r in failedResources.OrderBy(x => x.resource.Region).ThenBy(x => x.resource.Name))
            {
                Console.WriteLine($"{r.resource.Region} - {r.resource.Name}");
            }

            return new PolicySuiteRunResult();
        }

        private async Task<List<IResource>> LoadAWSResourcesAsync(IEnumerable<string> regions, List<Type> types, CancellationToken cancellationToken)
        {
            var resources = new List<IResource>();
            var account = await new AWSAccountIdentityProvider().GetAccountAsync(cancellationToken);

            foreach (var region in regions)
            {
                foreach (var provider in _awsProviders)
                {
                    if (types.Contains(provider.Key))
                        resources.AddRange(await provider.Value.DiscoverResourcesAsync(account, region, cancellationToken));
                }
            }

            return resources;
        }


        private List<CompilationUnit> GetPolicies(PolicySuiteScope scope)
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
                var func = PolicyCompiler.GetFunctionForPolicy(context, out var resource);
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
        private static IEnumerable<CompilationUnit> LoadPoliciesByName(string policyName)
        {
            const string builtInAws = "builtin\\aws\\";
            const string builtInAzure = "builtin\\azure\\";

            if (policyName.StartsWith(builtInAws, StringComparison.OrdinalIgnoreCase))
            {
                var prefix = "Aspect.BuiltIn.AWS.";
                var name = policyName.Substring(builtInAws.Length);
                if (LoadResourcesFromAssemblyByKeyPrefix(prefix).TryGetValue(name, out var policy))
                    yield return new BuiltInResourceCompilationUnit(name, policy);
                else
                {
                    // TODO :: ERROR : Invalid built in policy
                }

                yield break;
            }

            if (policyName.StartsWith(builtInAzure, StringComparison.OrdinalIgnoreCase))
            {
                var prefix = "Aspect.BuiltIn.Azure.";
                var name = policyName.Substring(builtInAzure.Length);
                if (LoadResourcesFromAssemblyByKeyPrefix(prefix).TryGetValue(name, out var policy))
                    yield return new BuiltInResourceCompilationUnit(name, policy);
                else
                {
                    // TODO :: ERROR : Invalid built in policy
                }

                yield break;
            }

            if (File.GetAttributes(policyName).HasFlag(FileAttributes.Directory))
            {
                foreach (var file in Directory.EnumerateFiles(policyName, "*.policy", SearchOption.AllDirectories))
                    yield return new FileCompilationUnit(file);

                yield break;
            }

            if (File.Exists(policyName))
            {
                yield return new FileCompilationUnit(policyName);
                yield break;
            }

            // TODO :: ERROR : Invalid built in policy

            static Dictionary<string, string> LoadResourcesFromAssemblyByKeyPrefix(string prefix)
            {
                var assembly = typeof(IPolicySuiteRunner).Assembly;
                return assembly.GetManifestResourceNames()
                    .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(x => x.Substring(prefix.Length), x =>
                    {
                        // manifest stream cannot be null here, but the compiler doesn't know that
                        using var sr = new StreamReader(assembly.GetManifestResourceStream(x)!);
                        return sr.ReadToEnd();
                    }, StringComparer.OrdinalIgnoreCase);
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