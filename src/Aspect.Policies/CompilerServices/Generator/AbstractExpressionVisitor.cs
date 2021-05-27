using System;
using Aspect.Policies.CompilerServices.Expressions;
using Expression = System.Linq.Expressions.Expression;
using LParameterExpression = System.Linq.Expressions.ParameterExpression;

namespace Aspect.Policies.CompilerServices.Generator
{
    internal abstract class AbstractExpressionVisitor
    {
        public Expression Visit(AbstractExpression expression, LParameterExpression inputParameter, bool shouldNegate)
            => VisitExpression(expression, inputParameter, shouldNegate);

        protected Expression VisitExpression(AbstractExpression expression, LParameterExpression inputParameter, bool shouldNegate)
        {
            if (expression is StatementExpression se)
                return VisitExpression(se, inputParameter, shouldNegate);

            if (expression is AccessorExpression ae)
                return VisitExpression(ae, inputParameter, shouldNegate);

            if (expression is BinaryExpression be)
                return VisitExpression(be, inputParameter, shouldNegate);

            if (expression is ConstantExpression ce)
                return VisitExpression(ce, inputParameter, shouldNegate);

            if (expression is FunctionCallExpression fce)
                return VisitExpression(fce, inputParameter, shouldNegate);

            if (expression is ParameterExpression pe)
                return VisitExpression(pe, inputParameter, shouldNegate);

            throw new NotSupportedException($"The expression type {expression.GetType().Name} is not currently supported");
        }

        protected abstract Expression VisitExpression(StatementExpression expression, LParameterExpression inputParameter, bool shouldNegate);
        protected abstract Expression VisitExpression(AccessorExpression expression, LParameterExpression inputParameter, bool shouldNegate);
        protected abstract Expression VisitExpression(ConstantExpression expression, LParameterExpression inputParameter, bool shouldNegate);
        protected abstract Expression VisitExpression(ParameterExpression expression, LParameterExpression inputParameter, bool shouldNegate);
    }
}
