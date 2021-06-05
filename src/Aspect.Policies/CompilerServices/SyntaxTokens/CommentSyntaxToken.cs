using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class CommentSyntaxToken : SyntaxToken
    {
        public CommentSyntaxToken(int lineNumber, int position, CompilationUnit source, int length)
            : base(lineNumber, position, source)
        {
            Length = length;
        }
    }
}
