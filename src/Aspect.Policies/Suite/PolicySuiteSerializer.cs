using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Policies.Suite
{
    internal sealed class PolicySuiteSerializer : IPolicySuiteSerializer
    {
        public string Serialize(PolicySuite suite)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return serializer.Serialize(suite);
        }

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
