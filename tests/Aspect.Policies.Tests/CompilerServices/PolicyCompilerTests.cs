using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.Generator;
using FluentAssertions;
using Xunit;

namespace Aspect.Policies.Tests.CompilerServices
{
    public class PolicyCompilerTests
    {
        public const int TestTimeoutMs = 1000;

        public PolicyCompilerTests()
        {
            Types.AddResource("Test", typeof(TestResource));
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("input.Int16 == 16", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int16 >= 16", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int16 > 0", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int16 <= 16", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int16 < 100", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int32 == 32", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int64 == 64", ResourcePolicyExecution.Passed)]
        [InlineData("input.Name == \"Testing\"", ResourcePolicyExecution.Passed)]
        [InlineData("input.Nested.Name == \"Nested\"", ResourcePolicyExecution.Passed)]
        [InlineData("input.Int16 != 16", ResourcePolicyExecution.Failed)]
        [InlineData("input.Int32 != 32", ResourcePolicyExecution.Failed)]
        [InlineData("input.Int64 != 64", ResourcePolicyExecution.Failed)]
        [InlineData("input.Name != \"Testing\"", ResourcePolicyExecution.Failed)]
        [InlineData("input.Nested.Name != \"Nested\"", ResourcePolicyExecution.Failed)]
        public async Task WillExecuteFunctionAgainstSpecifiedProperties(string policyPart, ResourcePolicyExecution expected)
        {
            var policy = $@"resource ""Test""

validate {{
    {policyPart}
}}";
            var testObject = new TestResource
            {
                Int16 = 16,
                Int32 = 32,
                Int64 = 64,
                Name = "Testing"
            };
            var executor = await GetPolicyValidatorForPolicy(policy);

            executor(testObject).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("startsWith(input.Name, \"Te\")", ResourcePolicyExecution.Passed)]
        [InlineData("startsWith(input.Nested.Name, \"nest\", false)", ResourcePolicyExecution.Passed)]
        [InlineData("startsWith(input.Nested.Name, \"nest\", true)", ResourcePolicyExecution.Failed)]
        [InlineData("endsWith(input.Name, \"ing\")", ResourcePolicyExecution.Passed)]
        [InlineData("endsWith(input.Nested.Name, \"ed\", false)", ResourcePolicyExecution.Passed)]
        [InlineData("endsWith(input.Nested.Name, \"ED\", true)", ResourcePolicyExecution.Failed)]
        [InlineData("contains(input.Name, \"es\")", ResourcePolicyExecution.Passed)]
        [InlineData("contains(input.Nested.Name, \"este\", false)", ResourcePolicyExecution.Passed)]
        [InlineData("contains(input.Nested.Name, \"ESTE\", true)", ResourcePolicyExecution.Failed)]
        public async Task UsingFunctionsWorksAsExpected(string policyPart, ResourcePolicyExecution expected)
        {
            var policy = $@"resource ""Test""

validate {{
    {policyPart}
}}";
            var testObject = new TestResource
            {
                Int16 = 16,
                Int32 = 32,
                Int64 = 64,
                Name = "Testing"
            };
            var executor = await GetPolicyValidatorForPolicy(policy);

            executor(testObject).Should().Be(expected);
        }

        //[Theory(Timeout = TestTimeoutMs)]
        [Theory]
        [InlineData("input.Enumerable[_] == 1", ResourcePolicyExecution.Passed)]
        [InlineData("input.Nested.List[_].Values[_] == 1", ResourcePolicyExecution.Passed)]
        [InlineData("input.Nested.List[_].Values[_] == 5", ResourcePolicyExecution.Failed)]
        [InlineData("input.List[_] == 1", ResourcePolicyExecution.Passed)]
        [InlineData("input.Array[_] == 1", ResourcePolicyExecution.Passed)]
        public async Task UsingArrayIterationsAsExpected(string policyPart, ResourcePolicyExecution expected)
        {
            var policy = $@"resource ""Test""

validate {{
    {policyPart}
}}";
            var testObject = new TestResource
            {
                Enumerable = new [] { 1, 2, 3 },
                List = new List<int> { 1, 2, 3 },
                Array = new [] { 1, 2, 3 },
            };
            var executor = await GetPolicyValidatorForPolicy(policy);
            executor(testObject).Should().Be(expected);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task WillReturnNullResultWhenInputIsNull()
        {
            var policy = $@"resource ""Test""

validate {{
    input.Type == ""Test""
}}";
            var executor = await GetPolicyValidatorForPolicy(policy);
            executor(null!).Should().Be(ResourcePolicyExecution.Null);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task WillReturnSkippedByPolicyWhenIncludeConditionNotMet()
        {
            var policy = $@"resource ""Test""

include {{
    input.Name != ""Test""
}}

validate {{
    input.Type == ""Test""
}}";
            var executor = await GetPolicyValidatorForPolicy(policy);
            executor(new TestResource { Name = "Test" }).Should().Be(ResourcePolicyExecution.SkippedByPolicy);
        }

        [Fact(Timeout = TestTimeoutMs)]
        public async Task WillReturnSkippedByPolicyWhenExcludeConditionNotMet()
        {
            var policy = $@"resource ""Test""

exclude {{
    input.Name == ""Test""
}}

validate {{
    input.Type == ""Test""
}}";
            var executor = await GetPolicyValidatorForPolicy(policy);
            executor(new TestResource { Name = "Test" }).Should().Be(ResourcePolicyExecution.SkippedByPolicy);
        }


        internal static async Task<Func<IResource, ResourcePolicyExecution>> GetPolicyValidatorForPolicy(string policyDocument)
            => await GetPolicyValidatorForPolicy(policyDocument, out _);
        internal static Task<Func<IResource, ResourcePolicyExecution>> GetPolicyValidatorForPolicy(string policyDocument, out CompilationContext context)
        {
            // Task is needed to work around this issue: https://github.com/xunit/xunit/issues/2222
            var c = new CompilationContext(new SourceTextCompilationUnit(policyDocument));
            context = c;
            return Task.Run(() =>
            {
                var compiler = new PolicyCompiler();
                return compiler.GetFunctionForPolicy(c.Source);
            });
        }

    }
}
