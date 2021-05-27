using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.CompilerServices;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class ParserTests
    {
        public const int TestTimeoutMs = 1000;

        public ParserTests()
        {
            Types.AddResource("Test", typeof(TestResource));
        }


        internal static async Task<PolicyAst?> GetPolicyAstForDocument(string policyDocument)
            => await GetPolicyAstForDocument(policyDocument, out _);
        internal static Task<PolicyAst?> GetPolicyAstForDocument(string policyDocument, out CompilationContext context)
        {
            // Task is needed to work around this issue: https://github.com/xunit/xunit/issues/2222
            var c = new CompilationContext(new SourceTextCompilationUnit(policyDocument));
            context = c;
            return Task.Run(() =>
            {
                var tokens = Lexer.Instance.GetAllSyntaxTokens(c).ToList();
                return Parser.Instance.Parse(c, tokens);
            });
        }
    }
}
