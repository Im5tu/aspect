namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class UnderscoreSyntaxToken : SyntaxToken
    {
        public UnderscoreSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
        }
    }
}
