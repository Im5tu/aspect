using System;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Services
{
    internal class ExecutablePolicy
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
