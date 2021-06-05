using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal class LineEndingSyntaxToken : SyntaxToken
    {
        public LineEndingSyntaxToken(int lineNumber, int position, CompilationUnit source, int length = 1)
            : base(lineNumber, position, source)
        {
            Length = length;
        }
    }
}
