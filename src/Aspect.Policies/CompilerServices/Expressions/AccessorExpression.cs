using System.Reflection;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class AccessorExpression : AbstractExpression
    {
        public AbstractExpression Accessor { get; }
        public PropertyInfo Property { get; }

        public AccessorExpression(AbstractExpression accessor, PropertyInfo property)
        {
            Accessor = accessor;
            Property = property;
        }

        public override string ToString() => $"{Accessor}.{Property.Name}";
    }
}
