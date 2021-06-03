using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.Suite;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Commands
{
    internal class WatchCommand : WaitableCommand<WatchCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicySuiteValidator _policySuiteValidator;

        public WatchCommand(IPolicyCompiler policyCompiler, IPolicySuiteValidator policySuiteValidator)
        {
            _policyCompiler = policyCompiler;
            _policySuiteValidator = policySuiteValidator;
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
                        if (args.FullPath.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            _policyCompiler.IsPolicyFileValid(args.FullPath, out var cntx);
                            cntx.WriteCompilationResultToConsole();
                        }
                        else
                        {
                            var validationResult = _policySuiteValidator.Validate(LoadPolicySuiteFromString(File.ReadAllText(args.FullPath)));

                            var table = new Table();
                            table.AddColumns("Policy", "IsValid", "Errors");
                            table.AddRow(args.FullPath, validationResult.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, validationResult.Errors.Select(x => $"- {x}")));
                            AnsiConsole.Render(table);
                        }
                    };

                    waiter();
                });

            return 0;
        }

        private PolicySuite LoadPolicySuiteFromString(string policy)
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            return deserializer.Deserialize<PolicySuite>(policy);
        }
    }
}
