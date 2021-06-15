using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<IResource, List<string>> _failed = new();
        private int _resourceCount = 0;

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
                _resourceCount += 1;
                foreach (var policy in _policies)
                    if (policy.Evaluator(resource) == ResourcePolicyExecution.Failed)
                        _failed.GetOrAdd(resource, _ => new List<string>()).Add(policy.Source.GetPolicyName());
            }
        }

        internal async Task AddResources(IEnumerable<IResource> resources)
        {
            foreach (var resource in resources)
                await _channel.Writer.WriteAsync(resource);
        }

        internal async Task<EvaluationResult> CompleteAsync()
        {
            _channel.Writer.TryComplete();
            await _channel.Reader.Completion;
            return new EvaluationResult(_resourceCount, _failed);
        }
    }

    internal record EvaluationResult(int ResourceCount, IReadOnlyDictionary<IResource, List<string>> FailedResources);
}
