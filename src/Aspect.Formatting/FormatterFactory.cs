using System;
using System.Collections.Generic;
using System.Linq;
using Aspect.Abstractions;

namespace Aspect.Formatting
{
    internal sealed class FormatterFactory : IFormatterFactory
    {
        private readonly Dictionary<FormatterType,IFormatter> _formatters;

        public FormatterFactory(IEnumerable<IFormatter> formatters)
        {
            _formatters = formatters.ToDictionary(x => x.FormatterType, x => x);
        }

        public IFormatter GetFormatterFor(FormatterType type)
        {
            if (_formatters.TryGetValue(type, out var formatter))
                return formatter;

            throw new Exception($"Formatter for '{type.ToString()}' has not been registered");
        }
    }
}