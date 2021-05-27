using System.Collections.Generic;
using System.Linq;

namespace Aspect.Policies.CompilerServices.Expressions
{
    internal class StatementExpression : AbstractExpression
    {
        public IEnumerable<AbstractExpression> Expressions { get; }

        public StatementExpression(IEnumerable<AbstractExpression> expressions)
        {
            Expressions = expressions.ToList();
        }
    }
}
