namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class ParenthesisSyntaxToken : SyntaxToken, IBoundedSyntaxToken
    {
        public BracketPosition BracketPosition { get; }

        internal ParenthesisSyntaxToken(BracketPosition bracketPosition, int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            BracketPosition = bracketPosition;
        }
    }
}
