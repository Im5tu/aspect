using Aspect.Abstractions;
using Newtonsoft.Json;

namespace Aspect.Formatting
{
    internal sealed class JsonFormatter : IFormatter
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Newtonsoft.Json.Formatting.Indented,
        };

        public FormatterType FormatterType { get; } = FormatterType.Json;

        public string Format<T>(T entity)  where T : class
        {
            return JsonConvert.SerializeObject(entity, _settings);
        }
    }
}
