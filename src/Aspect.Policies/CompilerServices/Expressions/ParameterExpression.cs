using System;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class ParameterExpression : AbstractExpression
    {
        public Type Type { get; }
        public string Name { get; }

        public ParameterExpression(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public override string ToString() => Type.Name;
    }
}
