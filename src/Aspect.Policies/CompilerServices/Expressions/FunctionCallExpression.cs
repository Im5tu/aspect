using System.Reflection;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class FunctionCallExpression : AbstractExpression
    {
        public MethodInfo Method { get; }
        public AccessorExpression Accessor { get; }
        public ConstantExpression[] Arguments { get; }

        public FunctionCallExpression(MethodInfo method, AccessorExpression accessor, ConstantExpression[] arguments)
        {
            Method = method;
            Accessor = accessor;
            Arguments = arguments;
        }

        public override string ToString() => $"{Method.Name.LowerFirstLetter()}({Accessor}, {Arguments})";
    }
}
