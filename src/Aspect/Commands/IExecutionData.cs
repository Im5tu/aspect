using System;

namespace Aspect.Commands
{
    internal interface IExecutionData : IDisposable
    {
        IExecutionData CreateScope();
        T Get<T>(string key) where T : class;
        void Set<T>(string key, T value) where T : class;
        void Remove(string key);
    }
}