using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class GreaterThanOrEqualSyntaxToken : SyntaxToken
    {
        public GreaterThanOrEqualSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            Length = 2;
        }
    }
}
