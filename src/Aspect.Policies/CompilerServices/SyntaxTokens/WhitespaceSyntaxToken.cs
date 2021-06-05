using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal class WhitespaceSyntaxToken : SyntaxToken
    {
        public WhitespaceSyntaxToken(int lineNumber, int position, CompilationUnit source, int length = 1)
            : base(lineNumber, position, source)
        {
            Length = length;
        }
    }
}
