using System.Linq;
using Aspect.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Aspect.Formatting
{
    internal sealed class JsonFormatter : IFormatter
    {
        private readonly JsonSerializerSettings _settings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ContractResolver = new OrderedContractResolver()
        };

        public FormatterType FormatterType { get; } = FormatterType.Json;

        public string Format<T>(T entity)  where T : class
        {
            return JsonConvert.SerializeObject(entity, _settings);
        }

        internal class OrderedContractResolver : DefaultContractResolver
        {
            protected override System.Collections.Generic.IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
            }
        }
    }
}
