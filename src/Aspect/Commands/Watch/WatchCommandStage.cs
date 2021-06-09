using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.Suite;
using Spectre.Console;

namespace Aspect.Commands.Watch
{
    internal sealed class WatchCommandStage : WaitableCommandStage<WatchCommand.Settings>
    {
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public WatchCommandStage(IPolicyCompiler policyCompiler,
            IPolicySuiteValidator policySuiteValidator,
            IPolicySuiteSerializer policySuiteSerializer)
        {
            _policyCompiler = policyCompiler;
            _policySuiteValidator = policySuiteValidator;
            _policySuiteSerializer = policySuiteSerializer;
        }

        protected override int ExecuteCommand(IExecutionData data, WatchCommand.Settings settings, Action waiter)
        {
            var delay = settings.DelayInterval.GetValueOrDefault(500);

            AnsiConsole.Status()
                .AutoRefresh(true)
                .Start("Watching path: " + settings.Directory, cntx =>
                {
                    cntx.Refresh();
                    var files = new Dictionary<string, DateTime>();

                    using var watcher = new FileSystemWatcher(settings.Directory!, "*.*");
                    watcher.EnableRaisingEvents = true;
                    watcher.Changed += (sender, args) =>
                    {
                        if (args.ChangeType != WatcherChangeTypes.Changed ||
                            !(args.FullPath.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase) || args.FullPath.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase)))
                            return;

                        if (files.TryGetValue(args.FullPath, out var lastEdited))
                            if (DateTime.UtcNow.Subtract(lastEdited).TotalMilliseconds < delay)
                                return;

                        files[args.FullPath] = DateTime.UtcNow;
                        Thread.Sleep(250); // Sleep is to work around file access issues because multiple events fire

                        AnsiConsole.Clear();

                        if (File.Exists(args.FullPath))
                            AnsiConsole.MarkupLine($"[red]File has been deleted: {args.FullPath}[/]");
                        else
                        {
                            if (args.FullPath.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                            {
                                _policyCompiler.IsPolicyFileValid(args.FullPath, out var cntx);
                                cntx.WriteCompilationResultToConsole();
                            }
                            else
                            {
                                var validationResult = _policySuiteValidator.Validate(_policySuiteSerializer.Deserialize(File.ReadAllText(args.FullPath)));

                                var table = new Table();
                                table.AddColumns("Policy", "IsValid", "Errors");
                                table.AddRow(args.FullPath, validationResult.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, validationResult.Errors.Select(x => $"- {x}")));
                                AnsiConsole.Render(table);
                            }
                        }
                    };

                    waiter();
                });

            return 0;
        }
    }
}
