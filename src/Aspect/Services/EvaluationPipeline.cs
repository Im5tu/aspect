using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Services
{
    internal sealed class EvaluationPipeline
    {
        private readonly IEnumerable<ExecutablePolicy> _policies;
        private readonly Channel<IResource> _channel;
        private readonly Task _evaluator;
        private readonly List<PolicySuiteRunResult.FailedResource> _failed = new();

        public EvaluationPipeline(IEnumerable<ExecutablePolicy> policies)
        {
            _policies = policies;
            _channel = Channel.CreateUnbounded<IResource>();
            _evaluator = EvaluateResources();
        }

        private async Task EvaluateResources()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));

            await foreach (var resource in _channel.Reader.ReadAllAsync())
            {
                foreach (var policy in _policies)
                    if (policy.Evaluator(resource) == ResourcePolicyExecution.Failed)
                        _failed.Add(new PolicySuiteRunResult.FailedResource {Resource = resource, Source = policy.Source.GetPolicyName()});
            }
        }

        internal async Task AddResources(IEnumerable<IResource> resources)
        {
            foreach (var resource in resources)
                await _channel.Writer.WriteAsync(resource);
        }

        internal async Task<List<PolicySuiteRunResult.FailedResource>> CompleteAsync()
        {
            _channel.Writer.TryComplete();
            await _channel.Reader.Completion;
            return _failed;
        }
    }
}
