namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class FunctionCallExpression : AbstractExpression
    {
        public string FunctionName { get; }
        public AccessorExpression Accessor { get; }
        public ConstantExpression[] Arguments { get; }

        public FunctionCallExpression(string functionName, AccessorExpression accessor, ConstantExpression[] arguments)
        {
            FunctionName = functionName;
            Accessor = accessor;
            Arguments = arguments;
        }

        public override string ToString() => $"{FunctionName}({Accessor}, {Arguments})";
    }
}
