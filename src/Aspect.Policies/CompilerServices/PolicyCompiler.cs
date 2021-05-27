using System;
using System.Linq;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.Generator;

namespace Aspect.Policies.CompilerServices
{
    internal class PolicyCompiler
    {
        // TODO :: TASK :: Build execute path for an object

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

        public bool IsPolicyFileValid(string filename) => IsPolicyValid(new FileCompilationUnit(filename));
        public bool IsPolicyValid(string policy) => IsPolicyValid(new SourceTextCompilationUnit(policy));
        private bool IsPolicyValid(CompilationUnit source)
        {
            var context = new CompilationContext(source);
            var policy = BuildPolicy(context);

            if (policy is null)
            {
                Console.WriteLine("No policy returned!");
                return false;
            }

            Console.WriteLine("Include:");
            foreach(var i in policy.Include?.Expressions ?? Enumerable.Empty<AbstractExpression>())
                Console.WriteLine("    - " + i);

            Console.WriteLine("Exclude:");
            foreach(var i in policy.Exclude?.Expressions ?? Enumerable.Empty<AbstractExpression>())
                Console.WriteLine("    - " + i);

            Console.WriteLine("Validate:");
            foreach(var i in policy.Validation.Expressions)
                Console.WriteLine("    - " + i);

            return context.Errors.Count == 0;
        }

        private PolicyAst? BuildPolicy(CompilationContext context)
        {
            var tokens = Lexer.Instance.GetAllSyntaxTokens(context);
            return Parser.Instance.Parse(context, tokens);
        }
    }
}
