using System.Collections.Generic;
using System.Threading;

namespace Aspect.Commands
{
    internal sealed class ExecutionData : IExecutionData
    {
        private readonly IExecutionData? _parent;
        private readonly Dictionary<string, object> _data = new();

        internal static readonly AsyncLocal<IExecutionData> Current = new();

        public ExecutionData(IExecutionData? parent = null)
        {
            _parent = parent;
            Current.Value = this;
        }

        public IExecutionData CreateScope() => new ExecutionData(this);

        public T Get<T>(string key)
            where T : class
        {
            if (_data.TryGetValue(key, out var val))
                return (T)val;

            if (_parent is null)
                throw new KeyNotFoundException($"Key not present in context: {key}. Keys: {string.Join(", ", _data.Keys)}");

            return _parent.Get<T>(key);
        }

        public void Set<T>(string key, T value)
            where T : class => _data[key] = value;

        public void Remove(string key)
            => _data.Remove(key);

        public void Dispose()
        {
            if (_parent is { })
                Current.Value = _parent;
        }
    }
}