using System.Collections.Generic;
using System.Linq;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices
{
    internal class Lexer : ILexer
    {
        public IReadOnlyCollection<SyntaxToken> GetAllSyntaxTokens(CompilationContext context)
            => GetAllSyntaxTokensImpl(context).ToList();

        private IEnumerable<SyntaxToken> GetAllSyntaxTokensImpl(CompilationContext context)
        {
            var source = context.Source;
            var text = source.GetAllText();

            if (string.IsNullOrWhiteSpace(text))
            {
                context.RaiseWarning("CA-LEX-004", 0, 0);
                yield break;
            }

            var line = 0;
            var position = 0;

            for (var index = 0; index < text.Length; index++)
            {
                var token = GetNextSyntaxToken(text, index, line, position, context);
                if (token is null)
                    yield break;

                index += token.Length - 1; // 1 is added by the loop
                position += token.Length;

                if (!(token is WhitespaceSyntaxToken or CommentSyntaxToken))
                {
                    if (token is LineEndingSyntaxToken)
                    {
                        line += 1;
                        position = 0;
                    }

                    yield return token;
                }
            }

            yield return new EofSyntaxToken(line, position, source);
        }

        private SyntaxToken? GetNextSyntaxToken(string text, int index, int line, int position, CompilationContext context)
        {
            var source = context.Source;
            int i;
            var c = text[index];
            if (c is '\r')
            {
                if (NextCharMatches(text, index, '\n'))
                    return new LineEndingSyntaxToken(line, position, source, 2);

                return new LineEndingSyntaxToken(line, position, source);
            }
            if (c is '\n')
                return new LineEndingSyntaxToken(line, position, source);
            if (char.IsWhiteSpace(c))
                return new WhitespaceSyntaxToken(line, position, source);
            if (c == '(')
                return new ParenthesisSyntaxToken(BracketPosition.Start, line, position, source);
            if (c == ')')
                return new ParenthesisSyntaxToken(BracketPosition.End, line, position, source);
            if (c == '{')
                return new BraceSyntaxToken(BracketPosition.Start, line, position, source);
            if (c == '}')
                return new BraceSyntaxToken(BracketPosition.End, line, position, source);
            if (c == '[')
                return new BracketSyntaxToken(BracketPosition.Start, line, position, source);
            if (c == ']')
                return new BracketSyntaxToken(BracketPosition.End, line, position, source);
            if (c == '<')
            {
                if (NextCharMatches(text, index, '='))
                    return new LessThanOrEqualSyntaxToken(line, position, source);

                return new LessThanSyntaxToken(line, position, source);
            }
            if (c == '>')
            {
                if (NextCharMatches(text, index, '='))
                    return new GreaterThanOrEqualSyntaxToken(line, position, source);

                return new GreaterThanSyntaxToken(line, position, source);
            }
            if (c == '.')
                return new PeriodSyntaxToken(line, position, source);
            if (c == ',')
                return new SeparatorSyntaxToken(line, position, source);
            if (c == '=')
            {
                if (NextCharMatches(text, index, '='))
                    return new EqualsSyntaxToken(line, position, source);

                context.RaiseError("CA-LEX-001", line, position);
                return null;
            }
            if (c == '!')
            {
                if (NextCharMatches(text, index, '='))
                    return new DoesNotEqualSyntaxToken(line, position, source);

                context.RaiseError("CA-LEX-001", line, position);
                return null;
            }
            if (c == '*')
                return new StarSyntaxToken(line, position, source);
            if (c == '_')
                return new UnderscoreSyntaxToken(line, position, source);
            if (c == '#')
            {
                // comments
                i = index;
                do
                {
                    i += 1;
                } while (i < text.Length && !(text[i] is '\r' or '\n'));

                return new CommentSyntaxToken(line, position, source, i - index);
            }

            // numeric parsing
            var b = (byte) c;
            if (b is 45 or 46 || b is >= 48 and <= 57)
            {
                i = index;
                // a minus sign can only appear at the front
                if (b == 45)
                    i++;

                var containsPeriod = b == 46;
                while (i <= text.Length)
                {
                    if (i != text.Length)
                    {
                        var nc = (byte) text[i];
                        if (nc is >= 48 and <= 57)
                        {
                            i += 1;
                            continue;
                        }
                        if (nc == 46 && !containsPeriod)
                        {
                            containsPeriod = true;
                            i += 1;
                            continue;
                        }
                    }

                    var length = i - index;
                    if (length == 1 && b == 45)
                    {
                        // We can't have just a hyphen on its own
                        context.RaiseError("CA-LEX-005", line, position);
                        return null;
                    }

                    return new NumericValueSyntaxToken(text.Substring(index, length), line, position, source);
                }
            }

            // identifier handling
            if (b is >= 65 and <= 90 || b is >= 97 and <= 122 || c == '"')
            {
                return GetIdentifierSyntaxToken(text, index, line, position, context);
            }

            // If we can't handle the syntax, return null
            return null;
        }
        private bool NextCharMatches(string text, int index, char c) => index + 1 < text.Length && text[index + 1] == c;
        private SyntaxToken? GetIdentifierSyntaxToken(string text, int index, int line, int position, CompilationContext context)
        {
            var source = context.Source;
            var i = index;
            var c = text[i];
            if (c == '"')
            {
                i += 1;
                while (i < text.Length)
                {
                    c = text[i];
                    if (c == '\\')
                    {
                        if (NextCharMatches(text, i, '"'))
                            i += 2;
                        else
                        {
                            context.RaiseError("CA-LEX-002", line, position + (i - index));
                            return null;
                        }
                    }
                    else if (c == '"')
                    {
                        return new QuotedIdentifierSyntaxToken(text.Substring(index + 1, i - index - 1), line, position, source);
                    }
                    else if (c is '\r' or '\n')
                        break;
                    else
                        i += 1;
                }

                context.RaiseError("CA-LEX-003", line, position + (i - index));
                return null;
            }

            while (i < text.Length)
            {
                // https://www.utf8-chartable.de/unicode-utf8-table.pl?utf8=dec&unicodeinhtml=dec
                // 65-90    => A - Z
                // 97-122   => A - Z
                var nc = (byte) text[i];
                if (nc is >= 65 and <= 90 || nc is >= 97 and <= 122 || (i - index > 0 && nc is >= 48 and <= 57))
                {
                    i += 1;
                }
                else
                    break;
            }

            var length = i - index;
            return new IdentifierSyntaxToken(text.Substring(index, length), line, position, source);
        }
    }
}
