using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using FluentAssertions;
using Xunit;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class LexerTests
    {
        private const int TestTimeoutMs = 1000;

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData(default(string))]
        [InlineData("")]
        [InlineData("  ")]
        public async Task CanHandleEmptyDocument(string value)
        {
            var tokens = await GetTokensForPolicy(value, out var context);
            tokens.Count.Should().Be(0);
            context.Warnings.Count.Should().Be(1);
            context.Warnings.First().Code.Should().Be("CA-LEX-004");
        }

        #region "CharacterTests"

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("\r")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public async Task CanHandleLineEndings(string input)
        {
            var policy = $"validate {{{input}}}";
            var tokens = await GetTokensForPolicy(policy);
            tokens[2].Should().BeOfType<LineEndingSyntaxToken>();
            tokens[2].Length.Should().Be(input.Length);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandlePoliciesWithoutLineEndings()
        {
            var policy = "validate {}";
            (await GetTokensForPolicy(policy)).Should().HaveCount(4);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleParenthesis()
        {
            var policy = "contains()";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<ParenthesisSyntaxToken>().Which.BracketPosition.Should()
                .Be(BracketPosition.Start);
            tokens[2].Should().BeOfType<ParenthesisSyntaxToken>().Which.BracketPosition.Should()
                .Be(BracketPosition.End);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleBraces()
        {
            var policy = "contains{}";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<BraceSyntaxToken>().Which.BracketPosition.Should().Be(BracketPosition.Start);
            tokens[2].Should().BeOfType<BraceSyntaxToken>().Which.BracketPosition.Should().Be(BracketPosition.End);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleBrackets()
        {
            var policy = "contains[]";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<BracketSyntaxToken>().Which.BracketPosition.Should().Be(BracketPosition.Start);
            tokens[2].Should().BeOfType<BracketSyntaxToken>().Which.BracketPosition.Should().Be(BracketPosition.End);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandlePeriod()
        {
            var policy = "input.Something";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<PeriodSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleSeparator()
        {
            var policy = "input,Something";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<SeparatorSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleGreaterThan()
        {
            var policy = "input>2";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<GreaterThanSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleGreaterThanOrEqual()
        {
            var policy = "input>=2";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<GreaterThanOrEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleLessThan()
        {
            var policy = "input<2";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<LessThanSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleLessThanOrEqual()
        {
            var policy = "input<=2";
            var tokens = await GetTokensForPolicy(policy);
            tokens[1].Should().BeOfType<LessThanOrEqualSyntaxToken>();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("HelloWorld", "HelloWorld")]
        [InlineData("hello", "hello")]
        [InlineData("WORLD", "WORLD")]
        [InlineData("test123", "test123")]
        [InlineData("test*", "test")]
        [InlineData("test[", "test")]
        [InlineData("test(", "test")]
        [InlineData("test{", "test")]
        public async Task CanHandleWords(string phrase, string expected)
        {
            var tokens = await GetTokensForPolicy(phrase);
            tokens[0].Should().BeOfType<IdentifierSyntaxToken>().Which.Identifier.Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("\"HelloWorld\"", "HelloWorld")]
        [InlineData("\"hello\"", "hello")]
        [InlineData("\"WORLD\"", "WORLD")]
        [InlineData("\"test123\"", "test123")]
        [InlineData("\"test*\"", "test*")]
        [InlineData("\"test[\"", "test[")]
        [InlineData("\"test(\"", "test(")]
        [InlineData("\"test{\"", "test{")]
        [InlineData("\"test\\\"\"", "test\"")]
        public async Task CanHandleQuotedWords(string phrase, string expected)
        {
            var tokens = await GetTokensForPolicy(phrase);
            tokens[0].Should().BeOfType<QuotedIdentifierSyntaxToken>().Which.ParsedValue.Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("\"test\\n\"", "CA-LEX-002")] // 002 - Invalid escape sequence
        [InlineData("\"test\\\"", "CA-LEX-003")] // 003 - Escape sequence not completed
        public async Task CanHandleQuotedWordsWithErrors(string phrase, string expected)
        {
            var tokens = await GetTokensForPolicy(phrase, out var context);
            tokens.Count.Should().Be(0);
            context.Errors.Should().HaveCount(1);
            context.Errors.First().Code.Should().Be(expected);
        }


        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("123", 123, typeof(Int16))]
        [InlineData("-123", -123, typeof(Int16))]
        [InlineData("123.12", 123.12, typeof(decimal))]
        [InlineData("-123.12", -123.12, typeof(decimal))]
        public async Task CanHandleNumerics(string phrase, object expected, Type expectedType)
        {
            var tokens = await GetTokensForPolicy(phrase);
            tokens[0].Should().BeOfType<NumericValueSyntaxToken>();
            ((NumericValueSyntaxToken) tokens[0]).Type.Should().Be(expectedType);
            ((NumericValueSyntaxToken) tokens[0]).Value.Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("(input.Something, 123)", 123, typeof(Int16))]
        [InlineData("(input.Something,123)", 123, typeof(Int16))]
        [InlineData("(input.Something, -123)", -123, typeof(Int16))]
        [InlineData("(input.Something,-123)", -123, typeof(Int16))]
        [InlineData("(input.Something, 123.12)", 123.12, typeof(decimal))]
        [InlineData("(input.Something,123.12)", 123.12, typeof(decimal))]
        [InlineData("(input.Something, -123.12)", -123.12, typeof(decimal))]
        [InlineData("(input.Something,-123.12)", -123.12, typeof(decimal))]
        public async Task CanHandleNumericsInFunction(string phrase, object expected, Type expectedType)
        {
            var tokens = await GetTokensForPolicy(phrase);
            tokens[5].Should().BeOfType<NumericValueSyntaxToken>();
            ((NumericValueSyntaxToken) tokens[5]).Type.Should().Be(expectedType);
            ((NumericValueSyntaxToken) tokens[5]).Value.Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("-")] // TODO :: Add more from https://www.utf8-chartable.de/unicode-utf8-table.pl?utf8=dec&unicodeinhtml=dec
        public async Task CanHandleMiscAsErrors(string phrase)
        {
            var tokens = await GetTokensForPolicy(phrase, out var context);
            tokens.Count.Should().Be(0);
            context.Errors.Should().HaveCount(1);
            context.Errors.First().Code.Should().Be("CA-LEX-005");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleEquals()
        {
            var tokens = await GetTokensForPolicy("==");
            tokens[0].Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleNotEquals()
        {
            var tokens = await GetTokensForPolicy("!=");
            tokens[0].Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleAssignmentWithError()
        {
            var tokens = await GetTokensForPolicy("input=2", out var context);
            tokens.Count.Should().Be(1);
            context.Errors.First().Code.Should().Be("CA-LEX-001");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleNegationWithError()
        {
            var tokens = await GetTokensForPolicy("!input", out var context);
            tokens.Count.Should().Be(0);
            context.Errors.First().Code.Should().Be("CA-LEX-001");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleStarSymbol()
        {
            var tokens = await GetTokensForPolicy("*");
            tokens[0].Should().BeOfType<StarSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleUnderscoreSymbol()
        {
            var tokens = await GetTokensForPolicy("_");
            tokens[0].Should().BeOfType<UnderscoreSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleLineComment()
        {
            var policy = @"# Comment
validate";
            var tokens = await GetTokensForPolicy(policy);
            tokens.Count.Should().Be(3);
            tokens[0].Should().BeOfType<LineEndingSyntaxToken>();
            tokens[1].Should().BeOfType<IdentifierSyntaxToken>();
            tokens[2].Should().BeOfType<EofSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanHandleCommentOnLine()
        {
            var policy = @"validate # Comment";
            var tokens = await GetTokensForPolicy(policy);
            tokens.Count.Should().Be(2);
            tokens[0].Should().BeOfType<IdentifierSyntaxToken>();
            tokens[1].Should().BeOfType<EofSyntaxToken>();
        }

        #endregion

        #region CompleteDocuments

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CorrectNumberOfLinesGenerated()
        {
            // \x20 is a UTF8 whitespace char. When whitespace is at the end of a line, a EOL token not generated...
            var policy = "resource \"AwsSecurityGroup\"\x20\x20\r\n\x20\x20\r\ninclude {\x20\x20\r\n\x20\x20\r\n}";
            var tokens = await GetTokensForPolicy(policy);

            tokens.OfType<LineEndingSyntaxToken>().Count().Should().Be(4);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseResourceSection()
        {
            var policy = "resource \"AWS.SecurityGroup\"";
            var tokens = await GetTokensForPolicy(policy);
            var i = 0;

            tokens.Count.Should().Be(3);
            ValidateToken<IdentifierSyntaxToken>(tokens[i++], 1, 1, 8, t => t.Identifier.Should().Be("resource"));
            ValidateToken<QuotedIdentifierSyntaxToken>(tokens[i++], 10, 1, 19,
                t => t.ParsedValue.Should().Be("AWS.SecurityGroup"));
            ValidateToken<EofSyntaxToken>(tokens[i], policy.Length + 1, 1);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseValidationSection()
        {
            var lineEndingLength = Environment.NewLine.Length;
            var policy = @"validate {
    input.Type == ""AWS.SecurityGroup""
}";
            var tokens = await GetTokensForPolicy(policy);
            var i = 0;

            tokens.Count.Should().Be(11);

            // validate {
            ValidateToken<IdentifierSyntaxToken>(tokens[i++], 1, 1, 8);
            ValidateToken<BraceSyntaxToken>(tokens[i++], 10, 1);
            ValidateToken<LineEndingSyntaxToken>(tokens[i++], 11, 1, lineEndingLength);

            // input.Type == ""AWS.SecurityGroup""
            ValidateToken<IdentifierSyntaxToken>(tokens[i++], 5, 2, 5, t => t.Identifier.Should().Be("input"));
            ValidateToken<PeriodSyntaxToken>(tokens[i++], 10, 2);
            ValidateToken<IdentifierSyntaxToken>(tokens[i++], 11, 2, 4, t => t.Identifier.Should().Be("Type"));
            ValidateToken<EqualsSyntaxToken>(tokens[i++], 16, 2, 2);
            ValidateToken<QuotedIdentifierSyntaxToken>(tokens[i++], 19, 2, 19,
                t => t.ParsedValue.Should().Be("AWS.SecurityGroup"));
            ValidateToken<LineEndingSyntaxToken>(tokens[i++], 38, 2, lineEndingLength);

            // }
            ValidateToken<BraceSyntaxToken>(tokens[i++], 1, 3);
            ValidateToken<EofSyntaxToken>(tokens[i], 2, 3);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseFutureSyntax()
        {
            var policy = @"validate {
    input.Type == {""Something"": ""SomethingElse""}
}";

            (await GetTokensForPolicy(policy)).Count.Should().BeGreaterThan(0);
        }

    #endregion

        private static void ValidateToken<T>(SyntaxToken token, int position, int lineNumber, int length = 1, Action<T>? additionalChecks = null)
            where T : SyntaxToken
        {
            token.Should().BeOfType<T>();
            token.Position.Should().Be(position);
            token.LineNumber.Should().Be(lineNumber);
            token.Length.Should().Be(length);

            additionalChecks?.Invoke((T)token);
        }

        private static async Task<List<SyntaxToken>> GetTokensForPolicy(string policyDocument)
            => await GetTokensForPolicy(policyDocument, out _);
        private static Task<List<SyntaxToken>> GetTokensForPolicy(string policyDocument, out CompilationContext context)
        {
            // Task is needed to work around this issue: https://github.com/xunit/xunit/issues/2222
            var c = new CompilationContext(new SourceTextCompilationUnit(policyDocument));
            context = c;
            return Task.Run(() => Lexer.Instance.GetAllSyntaxTokens(c).ToList());
        }
    }
}
