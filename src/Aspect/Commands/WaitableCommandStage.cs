using System;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal abstract class WaitableCommandStage<T> : CommandStage<T>, IDisposable
        where T : CommandSettings
    {
        private readonly EventWaitHandle _waitHandle = new(false, EventResetMode.AutoReset);

        internal sealed override Task<int?> InvokeAsync(IExecutionData data, T settings)
        {
            Console.CancelKeyPress += SetCancelled;

            ExecuteCommand(data, settings, () =>
            {
                _waitHandle.WaitOne();
                Console.CancelKeyPress -= SetCancelled;
            });

            return Task.FromResult<int?>(0);
        }

        protected abstract int ExecuteCommand(IExecutionData data, T settings, Action waiter);

        private void SetCancelled(object? sender, EventArgs e)
        {
            _waitHandle.Set();
        }

        public void Dispose()
        {
            _waitHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
