using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aspect.Formatters
{
    public interface IFormatter
    {
        ValueTask FormatAsync<T>(T entity)  where T : class;
        ValueTask FormatAsync<T>(IEnumerable<T> entities) where T : class;
    }
}