using System.Collections.Generic;
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

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("Key", "Value", "Key", "Value", true)]
        [InlineData("Key", "Value", "Key", "value", false)]
        [InlineData("Key", "Value", "key", "Value", false)]
        [InlineData("Key", "Value", "key ", "Value", false)]
        [InlineData("Key", "Value", "key", "Value ", false)]
        public void MatchesWhenCaseSensitive(string key, string value, string testKey, string testValue, bool expected)
        {
            var collection = new Dictionary<string, string>
            {
                [key] = value
            };

            BuiltInFunctions.Contains(collection, testKey, testValue, true).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("Key", "Value", "Key", "Value", true)]
        [InlineData("Key", "Value", "Key", "value", true)]
        [InlineData("Key", "Value", "key", "Value", true)]
        [InlineData("Key", "Value", "key ", "Value", false)]
        [InlineData("Key", "Value", "key", "Value ", false)]
        public void MatchesWhenCaseInsensitive(string key, string value, string testKey, string testValue, bool expected)
        {
            var collection = new Dictionary<string, string>
            {
                [key] = value
            };

            BuiltInFunctions.Contains(collection, testKey, testValue, false).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("Key", "Key", true)]
        [InlineData("Key", "key", false)]
        [InlineData("Key", "KEY", false)]
        [InlineData("Key", "key ", false)]
        public void HasKeyWhenCaseSensitive(string key, string testKey, bool expected)
        {
            var collection = new Dictionary<string, string>
            {
                [key] = "value"
            };

            BuiltInFunctions.HasKey(collection, testKey, true).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("Key", "Key", true)]
        [InlineData("Key", "key", true)]
        [InlineData("Key", "KEY", true)]
        [InlineData("Key", "KEY ", false)]
        public void HasKeyWhenCaseInsensitive(string key, string testKey, bool expected)
        {
            var collection = new Dictionary<string, string>
            {
                [key] = "value"
            };

            BuiltInFunctions.HasKey(collection, testKey, false).Should().Be(expected);
        }

        [Theory(Timeout = TestTimeoutMs)]
        [InlineData("192.168.0.1", "^([0-9]{1,3}\\.){3}[0-9]{1,3}(\\/([0-9]|[1-2][0-9]|3[0-2]))?$", true)]
        [InlineData("192.168.0.1/", "^([0-9]{1,3}\\.){3}[0-9]{1,3}(\\/([0-9]|[1-2][0-9]|3[0-2]))?$", false)]
        public void MatchesRegex(string key, string regex, bool expected)
        {
            BuiltInFunctions.Matches(key, regex).Should().Be(expected);
        }
    }
}
