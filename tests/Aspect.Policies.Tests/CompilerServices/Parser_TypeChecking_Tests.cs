using System;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using FluentAssertions;
using Xunit;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class Parser_TypeChecking_Tests : ParserTests
    {
        [Fact(Timeout = TestTimeoutMs)]
        public async Task WhenResourceNotRegisteredThenCompilationError()
        {
            var policy = @$"resource ""Test2""
validate {{
    input.Int32 == {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().BeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-009");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt32AndInt16Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Int32 == {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt64AndInt16Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Int64 == {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForDecimalAndInt16Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Decimal == {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt64AndInt32Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Int64 == {Int32.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForDecimalAndInt32Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Decimal == {Int32.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForDecimalAndInt64Property()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.Decimal == {Int64.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }


        [Fact(Timeout = TestTimeoutMs)]
        public async Task EnsureThatPropertyExistsOnType()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.NotExistsOnType == 123
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-012").Should().BeTrue();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task EnsureThatPropertyExistsOnType_Collection()
        {
            var policy = @$"resource ""TestResource""
validate {{
    input.NotExistsOnType[_] == 123
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-012").Should().BeTrue();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("contains()")]
        [InlineData("contains(input)")]
        [InlineData("contains(1,2,3,4)")]
        [InlineData("startsWith()")]
        [InlineData("startsWith(input)")]
        [InlineData("startsWith(1,2,3,4)")]
        [InlineData("endsWith()")]
        [InlineData("endsWith(input)")]
        [InlineData("endsWith(1,2,3,4)")]
        [InlineData("notExists(input.Something,1)")]
        [InlineData("123(input.Something,1)")]
        public async Task EnsureThatFunctionExists(string func)
        {
            var policy = @$"resource ""TestResource""
validate {{
    {func}
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-014").Should().BeTrue();
        }
    }
}
