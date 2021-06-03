using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Policies.Suite
{
    internal sealed class PolicySuiteSerializer : IPolicySuiteSerializer
    {
        public PolicySuite Deserialize(string policySuite)
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<PolicySuite>(policySuite);
        }
    }
}
