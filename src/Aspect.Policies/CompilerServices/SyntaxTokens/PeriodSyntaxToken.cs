namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class PeriodSyntaxToken : SyntaxToken
    {
        public PeriodSyntaxToken(int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
        }
    }
}
