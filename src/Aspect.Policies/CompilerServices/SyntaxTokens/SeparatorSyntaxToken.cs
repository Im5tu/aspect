using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class SeparatorSyntaxToken : SyntaxToken
    {
        public SeparatorSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
        }
    }
}
