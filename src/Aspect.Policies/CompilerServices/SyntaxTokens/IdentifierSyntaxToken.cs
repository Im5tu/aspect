using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class IdentifierSyntaxToken : SyntaxToken
    {
        public string Identifier { get; }

        public IdentifierSyntaxToken(string identifier, int lineNumber, int position, CompilationUnit source)
            : base (lineNumber, position, source)
        {
            Identifier = identifier;
            Length = Identifier.Length;
        }
    }
}
