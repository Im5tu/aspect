using Aspect.Abstractions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Formatting
{
    public class YamlFormatter : IFormatter
    {
        private readonly ISerializer _serializer;
        public FormatterType FormatterType { get; } = FormatterType.Yaml;

        public YamlFormatter()
        {
            _serializer = new SerializerBuilder()
                .WithIndentedSequences()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();
        }

        public string Format<T>(T entity) where T : class
        {
            return _serializer.Serialize(entity);
        }
    }
}
