using Aspect.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Policies.Suite
{
    internal sealed class PolicySuiteSerializer : IPolicySuiteSerializer
    {
        private readonly IFormatterFactory _formatterFactory;

        public PolicySuiteSerializer(IFormatterFactory formatterFactory)
        {
            _formatterFactory = formatterFactory;
        }

        public string Serialize(PolicySuite suite)
        {
            return _formatterFactory.GetFormatterFor(FormatterType.Yaml).Format(suite);
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
