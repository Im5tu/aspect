using System;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Policies.CompilerServices
{
    internal interface IPolicyCompiler
    {
        Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source);
        Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source, out Type? resource);
        Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context);
        Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context, out Type? resource);
        string? GetResourceForPolicy(string filename);
        string? GetResourceForPolicy(CompilationUnit source);
        bool IsPolicyFileValid(string filename);
        bool IsPolicyFileValid(string filename, out CompilationContext context);
        bool IsPolicyValid(string policy);
        bool IsPolicyValid(string policy, out CompilationContext context);
        bool IsPolicyValid(CompilationUnit source);
        bool IsPolicyValid(CompilationUnit source, out CompilationContext context);
    }
}
