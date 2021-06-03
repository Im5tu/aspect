using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.CompilerServices;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class ParserTests
    {
        public const int TestTimeoutMs = 1000;

        internal static async Task<PolicyAst?> GetPolicyAstForDocument(string policyDocument)
            => await GetPolicyAstForDocument(policyDocument, out _);
        internal static Task<PolicyAst?> GetPolicyAstForDocument(string policyDocument, out CompilationContext context)
        {
            // Task is needed to work around this issue: https://github.com/xunit/xunit/issues/2222
            var c = new CompilationContext(new SourceTextCompilationUnit(policyDocument));
            context = c;
            return Task.Run(() =>
            {
                var lexer = new Lexer();
                var parser = new Parser(new TestResourceTypeLocator());

                var tokens = lexer.GetAllSyntaxTokens(c).ToList();
                return parser.Parse(c, tokens);
            });
        }
    }
}
