using System;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Policies.CompilerServices
{
    internal class PolicyCompiler
    {
        public static Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source)
            => GetFunctionForPolicy(source, out _);
        public static Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source, out Type? resource)
        {
            var context = new CompilationContext(source);
            return GetFunctionForPolicy(context, out resource);
        }
        public static Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context)
            => GetFunctionForPolicy(context, out _);
        public static Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context, out Type? resource)
        {
            resource = null;
            var policy = BuildPolicy(context);

            if (!context.IsValid || policy is null)
                return null;

            resource = policy.ResourceType;
            return new LinqExpressionGenerator().Generate(policy);
        }
        public static string? GetResourceForPolicyFile(string filename) => BuildPolicy(new CompilationContext(new FileCompilationUnit(filename)))?.Resource;
        public static bool IsPolicyFileValid(string filename) => IsPolicyValid(new FileCompilationUnit(filename), out _);
        public static bool IsPolicyFileValid(string filename, out CompilationContext context) => IsPolicyValid(new FileCompilationUnit(filename), out context);
        public static bool IsPolicyValid(string policy) => IsPolicyValid(new SourceTextCompilationUnit(policy), out _);
        public static bool IsPolicyValid(string policy, out CompilationContext context) => IsPolicyValid(new SourceTextCompilationUnit(policy), out context);
        private static bool IsPolicyValid(CompilationUnit source, out CompilationContext context)
        {
            context = new CompilationContext(source);
            var policy = BuildPolicy(context);

            if (policy is null)
                return false;

            return context.IsValid;
        }
        private static PolicyAst? BuildPolicy(CompilationContext context)
        {
            var tokens = Lexer.Instance.GetAllSyntaxTokens(context);
            return Parser.Instance.Parse(context, tokens);
        }
    }
}
