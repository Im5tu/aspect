using System.Collections.Generic;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices
{
    internal interface IParser
    {
        PolicyAst? Parse(CompilationContext context, IEnumerable<SyntaxToken> tokens);
    }
}