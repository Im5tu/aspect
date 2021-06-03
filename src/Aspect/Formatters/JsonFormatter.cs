using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Spectre.Console;

namespace Aspect.Formatters
{
    internal sealed class JsonFormatter : IFormatter
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
        };

        public ValueTask FormatAsync<T>(T entity)  where T : class
        {
            Console.WriteLine(JsonConvert.SerializeObject(entity, _settings));
            return ValueTask.CompletedTask;
        }

        public ValueTask FormatAsync<T>(IEnumerable<T> entities) where T : class
        {
            Console.WriteLine(JsonConvert.SerializeObject(entities, _settings));
            return ValueTask.CompletedTask;
        }
    }
}
