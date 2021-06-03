using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using Aspect.Extensions;
using Aspect.Policies.CompilerServices;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class WatchCommand : WaitableCommand<WatchCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;

        public WatchCommand(IPolicyCompiler policyCompiler)
        {
            _policyCompiler = policyCompiler;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] WatchCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Directory))
                return ValidationResult.Error("Please specify a directory to watch");

            return base.Validate(context, settings);
        }

        protected override int ExecuteCommand(CommandContext context, WatchCommandSettings settings, Action waiter)
        {
            var delay = settings.DelayInterval.GetValueOrDefault(500);

            AnsiConsole.Status()
                .AutoRefresh(true)
                .Start("Watching path: " + settings.Directory, cntx =>
                {
                    cntx.Refresh();
                    var files = new Dictionary<string, DateTime>();

                    using var watcher = new FileSystemWatcher(settings.Directory!, "*.policy");
                    watcher.EnableRaisingEvents = true;
                    watcher.Changed += (sender, args) =>
                    {
                        if (args.ChangeType != WatcherChangeTypes.Changed)
                            return;

                        if (files.TryGetValue(args.FullPath, out var lastEdited))
                            if (DateTime.UtcNow.Subtract(lastEdited).TotalMilliseconds < delay)
                                return;

                        files[args.FullPath] = DateTime.UtcNow;
                        Thread.Sleep(250); // Sleep is to work around file access issues because multiple events fire

                        AnsiConsole.Clear();
                        _policyCompiler.IsPolicyFileValid(args.FullPath, out var cntx);
                        cntx.WriteCompilationResultToConsole();
                    };

                    waiter();
                });

            return 0;
        }
    }
}
