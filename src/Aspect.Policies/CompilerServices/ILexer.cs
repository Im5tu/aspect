using System.Collections.Generic;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices
{
    internal interface ILexer
    {
        IReadOnlyCollection<SyntaxToken> GetAllSyntaxTokens(CompilationContext context);
    }
}