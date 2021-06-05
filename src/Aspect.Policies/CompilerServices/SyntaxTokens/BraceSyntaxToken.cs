using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class BraceSyntaxToken : SyntaxToken, IBoundedSyntaxToken
    {
        public BracketPosition BracketPosition { get; }

        internal BraceSyntaxToken(BracketPosition bracketPosition, int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            BracketPosition = bracketPosition;
        }
    }
}
