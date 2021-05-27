using System.Collections.Generic;
using System.Linq;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices
{
    internal abstract class AbstractParser
    {
        protected static IEnumerable<IReadOnlyList<SyntaxToken>> ParseBlock(CompilationContext context, IEnumerator<SyntaxToken> enumerator)
        {
            var tokens = FindTokensBetween<BraceSyntaxToken>(context, enumerator).ToList();

            foreach (var line in SplitOn<LineEndingSyntaxToken>(tokens))
                yield return line.ToList();
        }

        protected static IEnumerable<SyntaxToken> FindTokensBetween<T>(CompilationContext context, IEnumerable<SyntaxToken> tokens)
            where T : IBoundedSyntaxToken
        {
            using var enumerator = tokens.GetEnumerator();
            return FindTokensBetween<T>(context, enumerator).ToList();
        }
        protected static IEnumerable<SyntaxToken> FindTokensBetween<T>(CompilationContext context, IEnumerator<SyntaxToken> enumerator)
            where T : IBoundedSyntaxToken
        {
            var opened = false;
            while (enumerator.MoveNext() && enumerator.Current is { })
            {
                var token = enumerator.Current;
                if (token is T bst)
                {
                    if (bst.BracketPosition == BracketPosition.Start)
                    {
                        if (opened)
                        {
                            context.RaiseError("CA-PAR-001", token);
                            yield break;
                        }

                        opened = true;
                    }
                    else if (bst.BracketPosition == BracketPosition.End)
                    {
                        if (!opened)
                            context.RaiseError("CA-PAR-002", token);

                        yield break;
                    }
                }
                else
                    yield return token;
            }
        }
        protected static IEnumerable<IEnumerable<SyntaxToken>> SplitOn<T>(IEnumerable<SyntaxToken> tokens)
            where T : SyntaxToken
        {
            using var enumerator = tokens.GetEnumerator();
            while (enumerator.MoveNext() && enumerator.Current is { })
            {
                var result = ForwardUntilTokenTypeFound<T>(enumerator).ToList();
                if (result.Count > 0)
                    yield return result;
            }
        }
        protected static IEnumerable<SyntaxToken> ForwardUntilTokenTypeFound<T>(IEnumerator<SyntaxToken> e)
            where T : SyntaxToken
        {
            do
            {
                if (!(e.Current is T))
                    yield return e.Current;
                else
                    yield break;
            } while (e.MoveNext() && e.Current is { });
        }
    }
}
