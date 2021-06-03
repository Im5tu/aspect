using System;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Policies.CompilerServices
{
    internal class PolicyCompiler : IPolicyCompiler
    {
        private readonly ILexer _lexer;
        private readonly IParser _parser;

        public PolicyCompiler(ILexer lexer, IParser parser)
        {
            _lexer = lexer;
            _parser = parser;
        }

        public Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source)
            => GetFunctionForPolicy(source, out _);
        public Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationUnit source, out Type? resource)
        {
            var context = new CompilationContext(source);
            return GetFunctionForPolicy(context, out resource);
        }
        public Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context)
            => GetFunctionForPolicy(context, out _);
        public Func<IResource, ResourcePolicyExecution>? GetFunctionForPolicy(CompilationContext context, out Type? resource)
        {
            resource = null;
            var policy = BuildPolicy(context);

            if (!context.IsValid || policy is null)
                return null;

            resource = policy.ResourceType;
            return new LinqExpressionGenerator().Generate(policy);
        }
        public string? GetResourceForPolicyFile(string filename) => BuildPolicy(new CompilationContext(new FileCompilationUnit(filename)))?.Resource;
        public bool IsPolicyFileValid(string filename) => IsPolicyValid(new FileCompilationUnit(filename), out _);
        public bool IsPolicyFileValid(string filename, out CompilationContext context) => IsPolicyValid(new FileCompilationUnit(filename), out context);
        public bool IsPolicyValid(string policy) => IsPolicyValid(new SourceTextCompilationUnit(policy), out _);
        public bool IsPolicyValid(string policy, out CompilationContext context) => IsPolicyValid(new SourceTextCompilationUnit(policy), out context);
        private bool IsPolicyValid(CompilationUnit source, out CompilationContext context)
        {
            context = new CompilationContext(source);
            var policy = BuildPolicy(context);

            if (policy is null)
                return false;

            return context.IsValid;
        }
        private PolicyAst? BuildPolicy(CompilationContext context)
        {
            var tokens = _lexer.GetAllSyntaxTokens(context);
            return _parser.Parse(context, tokens);
        }
    }
}
