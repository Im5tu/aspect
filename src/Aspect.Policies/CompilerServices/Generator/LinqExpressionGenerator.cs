using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using BinaryExpression = Aspect.Policies.CompilerServices.Expressions.BinaryExpression;
using ConstantExpression = Aspect.Policies.CompilerServices.Expressions.ConstantExpression;
using LParameterExpression = System.Linq.Expressions.ParameterExpression;
using ParameterExpression = Aspect.Policies.CompilerServices.Expressions.ParameterExpression;

namespace Aspect.Policies.CompilerServices.Generator
{
    // TODO :: TASK :: fill this out properly

    internal class LinqExpressionGenerator : AbstractExpressionVisitor
    {
        public Func<IResource, ResourcePolicyExecution> Generate(PolicyAst policy)
        {
            // Set the expressions for the function body
            var expressions = new List<Expression>();

            // Setup arguments and variable to hold the correct type
            var resourceType = typeof(IResource);
            var inputParameter = Expression.Parameter(resourceType);
            var variableParameter = Expression.Variable(policy.ResourceType);
            var variableAssignment = Expression.Assign(variableParameter, Expression.Convert(inputParameter, policy.ResourceType));

            // Setup the return stuff
            var resultVariable = Expression.Variable(typeof(ResourcePolicyExecution), "result");
            var resultVariableAssignment = Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Failed));
            var returnTarget = Expression.Label(typeof(ResourcePolicyExecution), "result");
            var returnNull = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Null)), Expression.Return(returnTarget, resultVariable));
            var returnByType = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.SkippedByType)), Expression.Return(returnTarget, resultVariable));
            var returnByPolicy = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.SkippedByPolicy)), Expression.Return(returnTarget, resultVariable));
            var returnPassed = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Passed)), Expression.Return(returnTarget, resultVariable));
            var returnFailed = Expression.Block(Expression.Assign(resultVariable, Expression.Constant(ResourcePolicyExecution.Failed)), Expression.Return(returnTarget, resultVariable));

            // Ensure that we start the function body with the correct parameters
            expressions.Add(resultVariableAssignment);
            expressions.Add(variableAssignment);

            // Validate the type is what we expect
            var typeProperty = policy.ResourceType.GetProperty(nameof(IResource.Type));
            var skipIfNull = Expression.IfThen(Expression.Equal(variableParameter, Expression.Constant(null, typeof(object))), returnNull);
            var resourceEquality = Expression.IfThen(Expression.NotEqual(Expression.MakeMemberAccess(variableParameter, typeProperty!), Expression.Constant(policy.Resource)), returnByType);

            expressions.Add(skipIfNull);
            expressions.Add(resourceEquality);

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

        protected void GenerateExpressionsForStatement(StatementExpression? expression, List<Expression> expressions, LParameterExpression input, Expression returnFailed, Expression returnPassed, bool shouldNegate)
        {
            if (expression is null)
                return;

            foreach (var statement in expression.Expressions)
            {
                if (statement is FunctionCallExpression fce)
                    Visit(fce, expressions, input, returnPassed, shouldNegate);
                else if (statement is BinaryExpression be)
                    Visit(be, expressions, input, returnFailed, returnPassed, shouldNegate);
            }
        }

        private void Visit(FunctionCallExpression expression, List<Expression> expressions, LParameterExpression input, Expression returnPassed, bool shouldNegate)
        {
            var arguments = expression.Arguments.Select(x => VisitExpression(x, input, shouldNegate)).ToArray();
            var method = expression.Method;
            var accessor = VisitExpression(expression.Accessor, input, shouldNegate);

            Expression functionCallExpression = Expression.Call(method, new[] {accessor}.Concat(arguments));

            if (shouldNegate)
                expressions.Add(Expression.IfThen(functionCallExpression, returnPassed));
            else
                expressions.Add(Expression.IfThen(functionCallExpression, returnPassed));
        }

        private void Visit(BinaryExpression expression, List<Expression> expressions, LParameterExpression input, Expression returnFailed, Expression returnPassed, bool shouldNegate)
        {
            // Build the right handside just in case we need to use it in a foreach
            var right = Visit(expression.Right, input, shouldNegate);
            Func<Expression, Expression> func;

            if (shouldNegate)
            {
                func = expression.OperatorToken switch
                {
                    EqualsSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.Equal(l, right)), returnPassed),
                    DoesNotEqualSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.NotEqual(l, right)), returnPassed),
                    GreaterThanSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.GreaterThan(l, right)), returnPassed),
                    GreaterThanOrEqualSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.GreaterThanOrEqual(l, right)), returnPassed),
                    LessThanSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.LessThan(l, right)), returnPassed),
                    LessThanOrEqualSyntaxToken => l => Expression.IfThen(Expression.Not(Expression.LessThanOrEqual(l, right)), returnPassed),
                    _ => throw new NotSupportedException()
                };
            }
            else
            {
                func = expression.OperatorToken switch
                {
                    EqualsSyntaxToken => l => Expression.IfThen(Expression.Equal(l, right), returnPassed),
                    DoesNotEqualSyntaxToken => l => Expression.IfThen(Expression.NotEqual(l, right), returnPassed),
                    GreaterThanSyntaxToken => l => Expression.IfThen(Expression.GreaterThan(l, right), returnPassed),
                    GreaterThanOrEqualSyntaxToken => l => Expression.IfThen(Expression.GreaterThanOrEqual(l, right), returnPassed),
                    LessThanSyntaxToken => l => Expression.IfThen(Expression.LessThan(l, right), returnPassed),
                    LessThanOrEqualSyntaxToken => l => Expression.IfThen(Expression.LessThanOrEqual(l, right), returnPassed),
                    _ => throw new NotSupportedException()
                };
            }

            // detect whether we need to build a collection accessor or not
            var requiresForEach = false;
            AbstractExpression leftExp = expression.Left;
            while (leftExp is AccessorExpression ae && !requiresForEach)
            {
                if (leftExp is CollectionAccessorExpression)
                    requiresForEach = true;
                else
                    leftExp = ae.Accessor;
            }

            Expression result;

            if (requiresForEach)
                result = VisitAsCollection(input, expression.Left, func, returnFailed, shouldNegate);
            else
                result = func(Visit(expression.Left, input, shouldNegate));

            expressions.Add(result);
        }


        protected override Expression VisitExpression(StatementExpression expression, LParameterExpression inputParameter, bool shouldNegate)
        {
            Expression? result = null;
            foreach (var exp in expression.Expressions)
            {
                var vistedExp = VisitExpression(exp, inputParameter, shouldNegate);

                if (result is null)
                    result = vistedExp;
                else
                    result = Expression.AndAlso(result, vistedExp);
            }

            result ??= Expression.Equal(Expression.Constant(true), Expression.Constant(false)); // TODO :: ERR
            return result;
        }

        protected override Expression VisitExpression(AccessorExpression expression, LParameterExpression inputParameter, bool shouldNegate)
        {
            return Expression.MakeMemberAccess(VisitExpression(expression.Accessor, inputParameter, shouldNegate), expression.Property);
        }

        private Expression VisitAsCollection(LParameterExpression inputParameter, AccessorExpression left, Func<Expression, Expression> evaluator, Expression returnFailed, bool shouldNegate)
        {
            // build a stack so that we can build the foreach loop from the start, in case things are nested
            AbstractExpression leftExp = left;
            while (leftExp is AccessorExpression ae)
            {
                //accessorStack.Push(ae);
                var nextEval = evaluator;
                if (ae is CollectionAccessorExpression cae)
                {
                    var loopVar = Expression.Variable(cae.Property.PropertyType.GetGenericArguments()[0]);
                    var prop = cae;
                    evaluator = input => ForEach(Expression.MakeMemberAccess(input, prop.Property), loopVar, nextEval(loopVar));
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
            //return Expression.Not(Expression.IsTrue(Expression.Constant(false)));
        }

        protected override Expression VisitExpression(ConstantExpression expression, LParameterExpression inputParameter, bool shouldNegate)
        {
            return Expression.Constant(expression.Value, expression.Type);
        }

        protected override Expression VisitExpression(ParameterExpression expression, LParameterExpression inputParameter, bool shouldNegate) => inputParameter;

        private Expression ForEach(Expression collection, LParameterExpression loopVar, Expression loopContent)
        {
            var elementType = loopVar.Type;
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            var enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            var enumeratorVar = Expression.Variable(enumeratorType);
            var getEnumeratorCall = Expression.Call(collection, enumerableType.GetMethod("GetEnumerator")!);
            var enumeratorAssign = Expression.Assign(enumeratorVar, getEnumeratorCall);

            // The MoveNext method's actually on IEnumerator, not IEnumerator<T>
            var moveNextCall = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext")!);

            var breakLabel = Expression.Label();

            var loop = Expression.Block(new[] { enumeratorVar },
                enumeratorAssign,
                Expression.Loop(
                    Expression.IfThenElse(
                        Expression.Equal(moveNextCall, Expression.Constant(true)),
                        Expression.Block(new[] { loopVar },
                            Expression.Assign(loopVar, Expression.Property(enumeratorVar, "Current")),
                            loopContent
                        ),
                        Expression.Break(breakLabel)
                    ),
                    breakLabel)
            );

            return loop;
        }
    }
}
