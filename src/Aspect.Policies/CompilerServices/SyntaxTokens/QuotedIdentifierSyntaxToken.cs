using System;
using Aspect.Policies.CompilerServices.CompilationUnits;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class QuotedIdentifierSyntaxToken : SyntaxToken
    {
        public string Value { get; }
        public string ParsedValue => Value.Replace("\\\"", "\"", StringComparison.Ordinal);

        public QuotedIdentifierSyntaxToken(string value, int lineNumber, int position, CompilationUnit source)
            : base (lineNumber, position, source)
        {
            Value = value;
            Length = Value.Length + 2;
        }
    }
}
