using System;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class ConstantExpression : AbstractExpression
    {
        public Type Type { get; }
        public object Value { get; }

        public ConstantExpression(Type type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString() => $"{Value}";
    }
}
