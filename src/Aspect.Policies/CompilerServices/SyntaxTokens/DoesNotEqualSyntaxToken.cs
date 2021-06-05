using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class DoesNotEqualSyntaxToken : SyntaxToken
    {
        public DoesNotEqualSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            Length = 2;
        }
    }
}
