using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class LessThanSyntaxToken : SyntaxToken
    {
        public LessThanSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
        }
    }
}
