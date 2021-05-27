using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class BinaryExpression  : AbstractExpression
    {
        public AccessorExpression Left { get; }
        public SyntaxToken OperatorToken { get; }
        public AbstractExpression Right { get; }

        public BinaryExpression(AccessorExpression left, SyntaxToken operatorToken, AbstractExpression right)
        {
            Left = left;
            Right = right;
            OperatorToken = operatorToken;
        }

        public override string ToString()
        {
            var symbol = OperatorToken switch
            {
                EqualsSyntaxToken => "==",
                DoesNotEqualSyntaxToken => "!=",
                GreaterThanSyntaxToken => ">",
                GreaterThanOrEqualSyntaxToken => ">=",
                LessThanSyntaxToken => ">=",
                LessThanOrEqualSyntaxToken => ">=",
                _ => $"UNKNOWN[{OperatorToken.GetType().Name}]"
            };

            return $"{Left} {symbol} {Right}";
        }
    }
}
