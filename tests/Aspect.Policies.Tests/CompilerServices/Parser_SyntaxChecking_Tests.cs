using System;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.CompilerServices.Expressions;
using Aspect.Policies.CompilerServices.SyntaxTokens;
using FluentAssertions;
using Xunit;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class Parser_SyntaxChecking_Tests : ParserTests
    {
        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseBasicOfSyntax()
        {
            var policy = @"resource ""Test""

include {
    input.Type == ""Test""
}

exclude {
    input.Type == ""Test""
}

validate {
    input.Type == ""Test""
}";

            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Exclude.Should().NotBeNull();
            ast.Include.Should().NotBeNull();
            ast.Validation.Should().NotBeNull();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseBasicOfSyntaxWhenIncludeMissing()
        {
            var policy = @"resource ""Test""

exclude {
    input.Type == ""Test""
}

validate {
    input.Type == ""Test""
}";

            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Exclude.Should().NotBeNull();
            ast.Include.Should().BeNull();
            ast.Validation.Should().NotBeNull();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseBasicOfSyntaxWhenIncludeEmpty()
        {
            var policy = @"resource ""Test""

include {}

exclude {
    input.Type == ""Test""
}

validate {
    input.Type == ""Test""
}";

            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Exclude.Should().NotBeNull();
            ast.Include.Should().BeNull();
            ast.Validation.Should().NotBeNull();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseBasicOfSyntaxWhenExcludeMissing()
        {
            var policy = @"resource ""Test""

include {
    input.Type == ""Test""
}

validate {
    input.Type == ""Test""
}";

            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Exclude.Should().BeNull();
            ast.Include.Should().NotBeNull();
            ast.Validation.Should().NotBeNull();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseBasicOfSyntaxWhenExcludeEmpty()
        {
            var policy = @"resource ""Test""

exclude {}

include {
    input.Type == ""Test""
}

validate {
    input.Type == ""Test""
}";

            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Exclude.Should().BeNull();
            ast.Include.Should().NotBeNull();
            ast.Validation.Should().NotBeNull();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("exclude", "input.Type == 123")]
        [InlineData("exclude", "startsWith(input.Type, \"AWS\")")]
        [InlineData("include", "input.Type == 123")]
        [InlineData("include", "startsWith(input.Type, \"AWS\")")]
        public async Task CanParseBasicExpressionForSection(string section, string expression)
        {
            var policy = $@"resource ""Test""

{section} {{
    {expression}
}}

validate {{
    input.Type == ""Test""
}}";
            var ast = await GetPolicyAstForDocument(policy);
            ast.Should().NotBeNull();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForStringProperty()
        {
            var policy = @"resource ""Test""
validate {
    input.Type == ""Test""
}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForStringProperty_WhenTypesDifferent()
        {
            var policy = @"resource ""Test""
validate {
    input.Type == 123
}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForStringProperty()
        {
            var policy = @"resource ""Test""
validate {
    input.Type != ""Test""
}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForStringProperty_WhenTypesDifferent()
        {
            var policy = @"resource ""Test""
validate {
    input.Type != 123
}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt16Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int16 == {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt16Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int16 == ""{Int16.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt16Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int16 != {Int16.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt16Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int16 == ""{Int16.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt32Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int32 == {Int32.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt32Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int32 == ""{Int32.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt32Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int32 != {Int32.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt32Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int32 == ""{Int32.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt64Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int64 == {Int64.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForInt64Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int64 == ""{Int64.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt64Property()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int64 != {Int64.MaxValue}
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForInt64Property_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Int64 == ""{Int64.MaxValue}""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForDecimalProperty()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Decimal == {Decimal.MaxValue}.0
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<EqualsSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseEqualityForDecimalProperty_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Decimal == ""{Decimal.MaxValue}.0""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForDecimalProperty()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Decimal != {Decimal.MaxValue}.0
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Count().Should().Be(1);
            BinaryExpression be = (BinaryExpression) ast.Validation.Expressions.First();
            be.OperatorToken.Should().BeOfType<DoesNotEqualSyntaxToken>();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseNegativeEqualityForDecimalProperty_WhenTypesDifferent()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Decimal == ""{Decimal.MaxValue}.0""
}}";
            var ast = await GetPolicyAstForDocument(policy, out var context);

            ast.Should().NotBeNull();
            context.Errors.Should().HaveCount(1).And.Subject.First().Code.Should().Be("CA-PAR-008");
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task CanParseArrayAnySyntaxOnInput()
        {
            var policy = @$"resource ""Test""
validate {{
    input.Enumerable[_] == 0
}}";
            var ast = await GetPolicyAstForDocument(policy);

            ast.Should().NotBeNull();
            ast!.Validation.Expressions.Should().HaveCount(1);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task ErrorWhenOnlyInputIsSpecified()
        {
            var policy = @"resource ""Test""
validate {
    input == ""123""
}";
            _ = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-010").Should().BeTrue();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task ErrorWhenSomethingOtherThanInputSpecified()
        {
            var policy = @"resource ""Test""
validate {
    something.Something == ""123""
}";
            _ = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-011").Should().BeTrue();
        }


        [Fact(Timeout = TestTimeoutMs)]
        public async Task ErrorWhenCollectionSyntaxIsIncorrect()
        {
            var policy = @"resource ""Test""
validate {
    input.List[a] == ""123""
}";
            _ = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-013").Should().BeTrue();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task ErrorWhenNoConstantExpression()
        {
            var policy = @"resource ""Test""
validate {
    input.Type ==
}";
            _ = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-015").Should().BeTrue();
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task ErrorWhenUnsupportedConstantSyntax()
        {
            var policy = @"resource ""Test""
validate {
    input.Type == [123,456]
}";
            _ = await GetPolicyAstForDocument(policy, out var context);
            context.Errors.Any(x => x.Code == "CA-PAR-016").Should().BeTrue();
        }
    }
}
