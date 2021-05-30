﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using StatementBlock = System.Collections.Generic.IReadOnlyCollection<System.Collections.Generic.IReadOnlyList<Aspect.Policies.CompilerServices.SyntaxTokens.SyntaxToken>>;

namespace Aspect.Policies.CompilerServices
{
    // TODO :: TASK :: type checking for all the expression generation, we need to know whether the policy is assignable to the property type
    internal sealed class Parser : AbstractParser
    {
        public static Parser Instance { get; } = new();

        private Parser()
        {
        }

        public PolicyAst? Parse(CompilationContext context, IEnumerable<SyntaxToken> tokens)
        {
            var policy = ParseStatements(context, tokens);
            if (policy is null)
                return null;

            return GenerateAST(context, policy);
        }

        private PolicyAst? GenerateAST(CompilationContext context, PolicyStatement policy)
        {
            StatementExpression GetStatementExpression(StatementBlock statements, Type type)
            {
                var expressions = new List<AbstractExpression>();
                foreach (var statement in statements)
                {
                    bool IsOperatorToken(SyntaxToken token) =>
                        token is DoesNotEqualSyntaxToken or EqualsSyntaxToken or
                            GreaterThanSyntaxToken or GreaterThanOrEqualSyntaxToken or LessThanSyntaxToken
                            or LessThanOrEqualSyntaxToken;

                    // Find the operator token
                    var index = -1;
                    for (var i = 1; i < statement.Count; i++)
                    {
                        if (IsOperatorToken(statement[i]))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index == statement.Count - 1)
                    {
                        context.RaiseError("CA-PAR-015", statement[^1]);
                        continue;
                    }

                    if (index == -1)
                    {
                        var functionCallExp = GetFunctionCallExpression(context, policy.Resource, statement, type);
                        if (functionCallExp is null)
                            continue;

                        expressions.Add(functionCallExp);
                    }
                    else if (index < statement.Count - 1)
                    {
                        var operatorToken = statement[index];

                        var left = BuildInputExpression(context,policy.Resource, statement.Take(index).ToList(), type);
                        var right = BuildConstantExpression(context, statement.Skip(index + 1).ToList());

                        if (left is null || right is null)
                            continue;

                        if (TryCoerseProperty(left.Property.PropertyType, right, out var newExpression))
                            right = newExpression;
                        else if (left.Property.PropertyType != right.Type)
                        {
                            context.RaiseError("CA-PAR-008", operatorToken);
                            continue;
                        }

                        expressions.Add(new BinaryExpression(left, operatorToken, right));
                    }
                }

                return new StatementExpression(expressions);
            }

            var type = Types.GetType(policy.Resource);
            if (type is null)
            {
                context.RaiseError("CA-PAR-009");
                return null;
            }

            var include = policy.IncludeStatements.Count == 0 ? null : GetStatementExpression(policy.IncludeStatements, type);
            var exclude = policy.ExcludeStatements.Count == 0 ? null : GetStatementExpression(policy.ExcludeStatements, type);
            var validate = GetStatementExpression(policy.ValidationStatements, type);

            return new PolicyAst(policy.Resource, type, include, exclude, validate);
        }

        private bool TryCoerseProperty(Type left, ConstantExpression right, [NotNullWhen(true)] out ConstantExpression? newExpression)
        {
            var type = right.Type;
            newExpression = null;

            if (left.IsAssignableTo(typeof(IEnumerable)))
            {
                var args = left.GetGenericArguments();
                if (args.Length == 1)
                    left = args[0];
                else if (args.Length == 2)
                    left = args[1];
                else
                    return false;
            }

            // We can rewrite the constant expression in certain cases before we do the type checking
            if (left == typeof(Int32))
            {
                if (type == typeof(Int16))
                {
                    newExpression = new ConstantExpression(typeof(Int32), Convert.ToInt32(right.Value));
                    return true;
                }
            }
            else if (left == typeof(Int64))
            {
                if (type == typeof(Int16)
                    || type == typeof(Int32))
                {
                    newExpression = new ConstantExpression(typeof(Int64), Convert.ToInt64(right.Value));
                    return true;
                }
            }
            else if (left == typeof(Decimal))
            {
                if (type == typeof(Int16)
                    || type == typeof(Int32)
                    || type == typeof(Int64))
                {
                    newExpression = new ConstantExpression(typeof(Decimal), Convert.ToDecimal(right.Value));
                    return true;
                }
            }

            return false;
        }

        private FunctionCallExpression? GetFunctionCallExpression(CompilationContext context, string resourceType, IReadOnlyList<SyntaxToken> tokens, Type type)
        {
            using var enumerator = tokens.GetEnumerator();

            if (enumerator.MoveNext() && enumerator.Current is IdentifierSyntaxToken ist)
            {
                var parameters = SplitOn<SeparatorSyntaxToken>(FindTokensBetween<ParenthesisSyntaxToken>(context, enumerator)).ToList();
                if (!BuiltInFunctions.TryLookupBuiltInFunction(ist.Identifier + parameters.Count, out var method))
                {
                    context.RaiseError("CA-PAR-014", ist);
                    return null;
                }

                var constExp = parameters.Skip(1).Select(x => BuildConstantExpression(context, x.ToList()) ?? throw new Exception("No expression built")).ToArray();
                var accessorExp = BuildInputExpression(context, resourceType, parameters[0].ToList(), type);

                if (accessorExp is null || method is null)
                    return null;

                return new FunctionCallExpression(method.Name, accessorExp, constExp);
            }

            context.RaiseError("CA-PAR-014", enumerator.Current);
            return null;
        }

        private ConstantExpression? BuildConstantExpression(CompilationContext context, IReadOnlyList<SyntaxToken> tokens)
        {
            if (tokens.Count == 1)
            {
                var token = tokens[0];

                if (token is QuotedIdentifierSyntaxToken qist)
                {
                    return new ConstantExpression(typeof(string), qist.ParsedValue);
                }

                if (token is NumericValueSyntaxToken nvst)
                {
                    return new ConstantExpression(nvst.Type, nvst.Value);
                }

                if (token is IdentifierSyntaxToken ist && (ist.Identifier.Equals("true", StringComparison.Ordinal) || ist.Identifier.Equals("false", StringComparison.Ordinal)))
                {
                    return new ConstantExpression(typeof(bool), bool.Parse(ist.Identifier));
                }
            }

            // TODO :: TASK :: Support arrays
            context.RaiseError("CA-PAR-016", tokens[^1]);
            return null;
        }

        private AccessorExpression? BuildInputExpression(CompilationContext context, string resourceType, IReadOnlyList<SyntaxToken> tokens, Type type)
        {
            if (tokens.Count < 3)
            {
                if (tokens.Count > 0)
                    context.RaiseError("CA-PAR-010", tokens[0]);
                else
                    context.RaiseError("CA-PAR-010");
                return null;
            }

            if (!(tokens[0] is IdentifierSyntaxToken {Identifier: "input"} id))
            {
                context.RaiseError("CA-PAR-011", tokens[0]);
                return null;
            }

            AbstractExpression accessor = new ParameterExpression(type, id.Identifier);
            foreach (var sequence in SplitOn<PeriodSyntaxToken>(tokens.Skip(2)))
            {
                var set = sequence.ToList();
                if (set.Count == 1)
                {
                    // We've already checked this above -> CA-PAR-011
                    var ist = (IdentifierSyntaxToken)set[0];
                    var property = type.GetProperty(ist.Identifier);
                    if (property is null)
                    {
                        context.RaiseError("CA-PAR-012", ist);
                        return null;
                    }

                    accessor = new AccessorExpression(accessor, property);
                    type = property.PropertyType;
                }
                else if (set.Count == 4)
                {
                    if (!(
                            set[0] is IdentifierSyntaxToken ist
                            && set[1] is BracketSyntaxToken {BracketPosition: BracketPosition.Start} && set[3] is BracketSyntaxToken {BracketPosition: BracketPosition.End}
                            && set[2] is NumericValueSyntaxToken or StarSyntaxToken or UnderscoreSyntaxToken
                    ))
                    {
                        context.RaiseError("CA-PAR-013", set[1]);
                        return null;
                    }

                    if (set[2] is StarSyntaxToken est)
                    {
                        var property = type.GetProperty(ist.Identifier);
                        if (property is null)
                        {
                            context.RaiseError("CA-PAR-012", ist);
                            return null;
                        }

                        accessor = new CollectionAccessorExpression(accessor, property, CollectionAccessorExpression.IndexerMode.Everything, est);
                        type = property.PropertyType.GetGenericArguments()[0];
                    }
                    else if (set[2] is UnderscoreSyntaxToken alost)
                    {
                        // TODO :: TASK :: refactor this repetitiveness
                        var property = type.GetProperty(ist.Identifier);
                        if (property is null)
                        {
                            context.RaiseError("CA-PAR-012", ist);
                            return null;
                        }

                        accessor = new CollectionAccessorExpression(accessor, property, CollectionAccessorExpression.IndexerMode.AtLeastOne, alost);
                        type = property.PropertyType.GetGenericArguments()[0];
                    }
                    else if (set[2] is NumericValueSyntaxToken nvst && nvst.Type == typeof(int))
                    {
                        var property = type.GetProperty(ist.Identifier);
                        if (property is null)
                        {
                            context.RaiseError("CA-PAR-012", ist);
                            return null;
                        }

                        accessor = new CollectionAccessorExpression(accessor, property, CollectionAccessorExpression.IndexerMode.IndexValue, nvst);
                        type = property.PropertyType.GetGenericArguments()[0];
                    }
                }
            }

            return accessor as AccessorExpression;
        }

        private static PolicyStatement? ParseStatements(CompilationContext context, IEnumerable<SyntaxToken> tokens)
        {
            using var enumerator = tokens.GetEnumerator();
            StatementBlock includeStatements = new List<IReadOnlyList<SyntaxToken>>();
            StatementBlock excludeStatements = new List<IReadOnlyList<SyntaxToken>>();
            StatementBlock validationStatements = new List<IReadOnlyList<SyntaxToken>>();
            string? resource = null;

            while (enumerator.MoveNext() && enumerator.Current is { })
            {
                var token = enumerator.Current;
                if (token is IdentifierSyntaxToken id)
                {
                    switch (id.Identifier)
                    {
                        case "include":
                            includeStatements = ParseBlock(context, enumerator).ToList();
                            break;
                        case "exclude":
                            excludeStatements = ParseBlock(context, enumerator).ToList();
                            break;
                        case "validate":
                            validationStatements = ParseBlock(context, enumerator).ToList();
                            break;
                        case "resource":
                            if (enumerator.MoveNext() && enumerator.Current is QuotedIdentifierSyntaxToken ist)
                                resource = ist.ParsedValue;
                            else
                            {
                                context.RaiseError("CA-PAR-006", id.LineNumber, id.Position + id.Length + 1);
                                return null;
                            }
                            break;
                        default:
                            context.RaiseError("CA-PAR-005", token); // Invalid top level identifier
                            break;
                    }
                }
                else if (!token.IsTerminationToken)
                    context.RaiseError("CA-PAR-003", token); // Invalid token
            }

            if (validationStatements.Count == 0)
            {
                context.RaiseError("CA-PAR-004"); // No validation statements
                return null;
            }

            if (resource is null)
            {
                context.RaiseError("CA-PAR-007"); // No resource defined in document
                return null;
            }

            return new PolicyStatement(resource, includeStatements, excludeStatements, validationStatements);
        }

        public record PolicyStatement(string Resource, StatementBlock IncludeStatements, StatementBlock ExcludeStatements, StatementBlock ValidationStatements);
    }
}