namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal abstract class SyntaxToken
    {
        public CompilationUnit Source { get; }
        public int Position => PositionRaw + 1;
        internal int PositionRaw { get; }
        public int LineNumber => LineNumberRaw + 1;
        internal int LineNumberRaw { get; }
        public int Length { get; protected set; }
        public bool IsTerminationToken => this is EofSyntaxToken || this is LineEndingSyntaxToken;

        protected SyntaxToken(int lineNumber, int position, CompilationUnit source)
        {
            Source = source;
            Length = 1;
            LineNumberRaw = lineNumber;
            PositionRaw = position;
        }
    }
}
