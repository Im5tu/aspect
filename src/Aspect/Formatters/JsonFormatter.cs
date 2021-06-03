using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspect.Formatters
{
    internal sealed class JsonFormatter : IFormatter
    {
        public ValueTask FormatAsync<T>(T entity)  where T : class => FormatAsync(new[] {entity});
        public ValueTask FormatAsync<T>(IEnumerable<T> entities) where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}
