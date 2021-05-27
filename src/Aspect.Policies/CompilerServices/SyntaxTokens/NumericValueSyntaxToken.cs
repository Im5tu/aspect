using System;

namespace Aspect.Policies.CompilerServices.SyntaxTokens
{
    internal sealed class NumericValueSyntaxToken : SyntaxToken
    {
        public Type Type { get; }
        public object Value { get; }

        public NumericValueSyntaxToken(string value, int lineNumber, int position, CompilationUnit source)
            : base(lineNumber, position, source)
        {
            Length = value.Length;

            if (value.Contains(".", StringComparison.Ordinal))
            {
                Value = decimal.Parse(value);
                Type = typeof(decimal);
            }
            else
            {
                if (Int16.TryParse(value, out var i16))
                {
                    Value = i16;
                    Type = typeof(short);
                }
                else if (Int32.TryParse(value, out var i32))
                {
                    Value = i32;
                    Type = typeof(int);
                }
                else if (Int64.TryParse(value, out var i64))
                {
                    Value = i64;
                    Type = typeof(long);
                }
                else
                {
                    throw new ArgumentException($"Value: '{value}' cannot be converted to a valid number.");
                }
            }
        }
    }
}
