using System;
using Aspect.Policies.CompilerServices.Expressions;

namespace Aspect.Policies.CompilerServices
{
    internal record PolicyAst(string Resource, Type ResourceType, StatementExpression? Include, StatementExpression? Exclude, StatementExpression Validation);
}
