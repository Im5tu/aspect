using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class GreaterThanSyntaxToken : SyntaxToken
    {
        public GreaterThanSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
        }
    }
}
