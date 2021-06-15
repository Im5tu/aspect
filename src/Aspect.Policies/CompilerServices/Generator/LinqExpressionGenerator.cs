using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using BinaryExpression = Aspect.Policies.CompilerServices.Expressions.BinaryExpression;
using ConstantExpression = Aspect.Policies.CompilerServices.Expressions.ConstantExpression;
using LParameterExpression = System.Linq.Expressions.ParameterExpression;
using ParameterExpression = Aspect.Policies.CompilerServices.Expressions.ParameterExpression;

namespace Aspect.Policies.CompilerServices.Generator
{
    internal class LinqExpressionGenerator
    {
        public Func<IResource, ResourcePolicyExecution> Generate(PolicyAst policy)
        {
            // Set the expressions for the function body
            var expressions = new List<Expression>();

            // Setup the return stuff
            var resultVariable = Expression.Variable(typeof(ResourcePolicyExecution), "result");
            var resultVariableAssignment = Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Failed));
            var returnTarget = Expression.Label(typeof(ResourcePolicyExecution), "result");
            var returnNull = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Null)), Expression.Return(returnTarget, resultVariable));
            var returnByType = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.SkippedByType)), Expression.Return(returnTarget, resultVariable));
            var returnByPolicy = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.SkippedByPolicy)), Expression.Return(returnTarget, resultVariable));
            var returnPassed = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Passed)), Expression.Return(returnTarget, resultVariable));
            var returnFailed = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Failed)), Expression.Return(returnTarget, resultVariable));

            // Setup arguments and variable to hold the correct type
            var resourceType = typeof(IResource);
            var inputParameter = Expression.Parameter(resourceType);

            // Skip if null
            var skipIfNull = Expression.IfThen(Expression.Equal(inputParameter, Expression.Constant(null, typeof(object))), returnNull);
            expressions.Add(skipIfNull);

            // Variable assignment and type check
            var variableParameter = Expression.Variable(policy.ResourceType);
            var variableAssignment = Expression.IfThenElse(Expression.Not(Expression.TypeEqual(inputParameter, policy.ResourceType)), returnByType, Expression.Assign(variableParameter, Expression.Convert(inputParameter, policy.ResourceType)));

            // Ensure that we start the function body with the correct parameters
            expressions.Add(resultVariableAssignment);
            expressions.Add(variableAssignment);

            // Build the parts for the body, where applicable
            GenerateExpressionsForStatement(policy.Include, expressions, variableParameter, returnFailed, returnByPolicy, true);
            GenerateExpressionsForStatement(policy.Exclude, expressions, variableParameter, returnFailed, returnByPolicy, false);
            GenerateExpressionsForStatement(policy.Validation, expressions, variableParameter, returnFailed, returnPassed, false);

            // Once we're done with the function body, add the result label
            expressions.Add(Expression.Label(returnTarget, resultVariable));

            // Put it all together
            var body = Expression.Block(typeof(ResourcePolicyExecution),
                new[] {resultVariable, variableParameter},
                expressions.ToArray());

            var compiled = Expression.Lambda<Func<IResource, ResourcePolicyExecution>>(
                body,
                new [] { inputParameter }
            ).Compile();

            return compiled;
        }

        private Expression Visit(AbstractExpression expression, LParameterExpression inputParameter)
        {
            if (expression is AccessorExpression ae)
                return VisitAccessor(ae, inputParameter);

            if (expression is BinaryExpression be)
                return VisitBinary(be, inputParameter);

            if (expression is ConstantExpression ce)
                return VisitConstant(ce);

            if (expression is ParameterExpression pe)
                return inputParameter;

            throw new NotSupportedException($"The expression type {expression.GetType().Name} is not currently supported");
        }

        private void GenerateExpressionsForStatement(StatementExpression? expression, List<Expression> expressions, LParameterExpression input, Expression returnFailed, Expression returnPassed, bool shouldNegate)
        {
            if (expression is null)
                return;


            Expression? condition = null;
            foreach (var statement in expression.Expressions)
            {
                Expression? exp = null;
                if (statement is FunctionCallExpression fce)
                    exp = VisitFunction(fce, input);
                else if (statement is BinaryExpression be)
                    exp = VisitBinary(be, input);

                if (exp is null)
                    continue;

                if (condition is null)
                    condition = exp;
                else
                    condition = Expression.AndAlso(condition, exp);
            }

            if (condition is null)
                return;

            if (shouldNegate)
                condition = Expression.Not(condition);

            expressions.Add(Expression.IfThen(condition, returnPassed));
        }

        private Expression VisitFunction(FunctionCallExpression expression, LParameterExpression input)
        {
            var arguments = expression.Arguments.Select(x => Visit(x, input)).ToArray();
            var method = expression.Method;
            var accessor = VisitAccessor(expression.Accessor, input);

            return Expression.Call(method, new[] {accessor}.Concat(arguments));
        }

        private Expression VisitBinary(BinaryExpression expression, LParameterExpression input)
        {
            // Build the right handside just in case we need to use it in a foreach
            var right = Visit(expression.Right, input);
            Func<Expression, Expression> func = expression.OperatorToken switch
            {
                EqualsSyntaxToken => l => Expression.Equal(l.ValueOrDefault(), right.ValueOrDefault()),
                DoesNotEqualSyntaxToken => l => Expression.NotEqual(l.ValueOrDefault(), right.ValueOrDefault()),
                GreaterThanSyntaxToken => l => Expression.GreaterThan(l.ValueOrDefault(), right.ValueOrDefault()),
                GreaterThanOrEqualSyntaxToken => l => Expression.GreaterThanOrEqual(l.ValueOrDefault(), right.ValueOrDefault()),
                LessThanSyntaxToken => l => Expression.LessThan(l.ValueOrDefault(), right.ValueOrDefault()),
                LessThanOrEqualSyntaxToken => l => Expression.LessThanOrEqual(l.ValueOrDefault(), right.ValueOrDefault()),
                _ => throw new NotSupportedException()
            };

            // detect whether we need to build a collection accessor or not
            var requiresForEach = false;
            AbstractExpression leftExp = expression.Left;
            while (leftExp is AccessorExpression ae && !requiresForEach)
            {
                if (leftExp is CollectionAccessorExpression)
                {
                    requiresForEach = true;
                    break;
                }

                leftExp = ae.Accessor;
            }

            if (requiresForEach)
                return VisitCollection(input, expression.Left, func);

            return func(Visit(expression.Left, input));
        }

        private Expression VisitAccessor(AccessorExpression expression, LParameterExpression inputParameter)
        {
            return Expression.MakeMemberAccess(Visit(expression.Accessor, inputParameter), expression.Property);
        }

        private Expression VisitCollection(LParameterExpression inputParameter, AccessorExpression left, Func<Expression, Expression> evaluator)
        {
            // build a stack so that we can build the foreach loop from the start, in case things are nested
            AbstractExpression leftExp = left;
            while (leftExp is AccessorExpression ae)
            {
                var nextEval = evaluator;
                if (ae is CollectionAccessorExpression cae)
                {
                    var pt = cae.Property.PropertyType;
                    var loopPropertyType = pt.IsArray ? pt.GetElementType()! : pt.GetGenericArguments()[0];
                    var property = cae.Property;
                    var method = cae.Mode == CollectionAccessorExpression.IndexerMode.All ? nameof(Enumerable.All) : nameof(Enumerable.Any);
                    evaluator = input =>
                    {
                        var inputParam = Expression.Parameter(loopPropertyType);
                        var body = nextEval(inputParam);
                        var evalFunc = Expression.Lambda(typeof(Func<,>).MakeGenericType(loopPropertyType, typeof(bool)), body, inputParam);

                        return Expression.Call(typeof(Enumerable), method, new[] { loopPropertyType },
                             Expression.MakeMemberAccess(input, property), evalFunc
                         );
                    };
                }
                else
                {
                    var prop = ae;
                    evaluator = input =>
                    {
                        var newInput = Expression.MakeMemberAccess(input, prop.Property);
                        return nextEval(newInput);
                    };
                }
                leftExp = ae.Accessor;
            }

            return evaluator(inputParameter);
        }

        private Expression VisitConstant(ConstantExpression expression) => Expression.Constant(expression.Value, expression.Type);
    }

    internal static class ExpressionExtensions
    {
        internal static Expression ValueOrDefault(this Expression expression)
        {
            if (!(expression is System.Linq.Expressions.ConstantExpression or System.Linq.Expressions.MemberExpression))
                return expression;

            var nullableType = Nullable.GetUnderlyingType(expression.Type);
            if (nullableType is null)
                return expression;

            var method = expression.Type.GetMethod(nameof(Nullable<int>.GetValueOrDefault), Array.Empty<Type>())!;
            return Expression.Call(expression, method);
        }
    }
}
