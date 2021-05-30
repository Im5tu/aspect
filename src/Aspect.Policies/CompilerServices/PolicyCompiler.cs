using System;
using System.Linq;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Policies.CompilerServices
{
    internal class PolicyCompiler
    {
        public Func<IResource, ResourcePolicyExecution> GetFunctionForPolicy(CompilationUnit source)
        {
            var context = new CompilationContext(source);
            return GetFunctionForPolicy(context);
        }
        public Func<IResource, ResourcePolicyExecution> GetFunctionForPolicy(CompilationContext context)
        {
            var policy = BuildPolicy(context);
            return new LinqExpressionGenerator().Generate(policy!);
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

            return context.Errors.Count == 0;
        }

        private PolicyAst? BuildPolicy(CompilationContext context)
        {
            var tokens = Lexer.Instance.GetAllSyntaxTokens(context);
            return Parser.Instance.Parse(context, tokens);
        }
    }
}
