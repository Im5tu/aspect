using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal abstract class WaitableCommand<T> : Command<T>, IDisposable
        where T : CommandSettings
    {
        private readonly EventWaitHandle _waitHandle = new(false, EventResetMode.AutoReset);

        public sealed override int Execute([NotNull] CommandContext context, [NotNull] T settings)
        {
            Console.CancelKeyPress += SetCancelled;

            return ExecuteCommand(context, settings, () =>
            {
                _waitHandle.WaitOne();
                Console.CancelKeyPress -= SetCancelled;
            });
        }

        protected abstract int ExecuteCommand(CommandContext context, T settings, Action waiter);

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
