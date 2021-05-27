using FluentAssertions;
using Xunit;

namespace Aspect.Policies.Tests
{
    public class BuiltInFunctionsTests
    {
        public const int TestTimeoutMs = 1000;

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "es")]
        [InlineData("test", "ES")]
        [InlineData("TEst", "es")]
        [InlineData("teST", "es")]
        [InlineData("TEST", "es")]
        public void ShouldContainValueWhenCaseInsensitive(string input, string value)
        {
            BuiltInFunctions.Contains(input, value).Should().BeTrue();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "es", true)]
        [InlineData("test", "ES", false)]
        [InlineData("TEst", "es", false)]
        [InlineData("teST", "es", false)]
        [InlineData("TEST", "es", false)]
        public void ShouldContainValueWhenCaseSensitive(string input, string value, bool expected)
        {
            BuiltInFunctions.Contains(input, value, true).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "tes")]
        [InlineData("test", "TEs")]
        [InlineData("TEst", "TEs")]
        [InlineData("teST", "teS")]
        [InlineData("TEST", "tes")]
        public void StartsWithWhenCaseInsensitive(string input, string value)
        {
            BuiltInFunctions.StartsWith(input, value).Should().BeTrue();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "tes", true)]
        [InlineData("test", "TEs", false)]
        [InlineData("TEst", "TEs", true)]
        [InlineData("teST", "teS", true)]
        [InlineData("TEST", "tes", false)]
        public void StartsWithWhenCaseSensitive(string input, string value, bool expected)
        {
            BuiltInFunctions.StartsWith(input, value, true).Should().Be(expected);
        }


        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "est")]
        [InlineData("test", "EST")]
        [InlineData("TEst", "Est")]
        [InlineData("teST", "Est")]
        [InlineData("TEST", "est")]
        public void EndsWithWhenCaseInsensitive(string input, string value)
        {
            BuiltInFunctions.EndsWith(input, value).Should().BeTrue();
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("test", "est", true)]
        [InlineData("test", "EST", false)]
        [InlineData("TEst", "Est", true)]
        [InlineData("teST", "Est", false)]
        [InlineData("TEST", "est", false)]
        public void EndsWithWhenCaseSensitive(string input, string value, bool expected)
        {
            BuiltInFunctions.EndsWith(input, value, true).Should().Be(expected);
        }
    }
}
