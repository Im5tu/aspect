using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class BracketSyntaxToken : SyntaxToken, IBoundedSyntaxToken
    {
        public BracketPosition BracketPosition { get; }

        internal BracketSyntaxToken(BracketPosition bracketPosition, int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            BracketPosition = bracketPosition;
        }
    }
}
