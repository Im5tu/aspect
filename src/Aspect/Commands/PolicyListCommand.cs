using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Aspect.Policies.CompilerServices;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyListCommand : Command<PolicyListCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;

        public PolicyListCommand(IPolicyCompiler policyCompiler)
        {
            _policyCompiler = policyCompiler;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] PolicyListCommandSettings commandSettings)
        {
            if (!string.IsNullOrWhiteSpace(commandSettings.Directory) && !Directory.Exists(commandSettings.Directory))
                return ValidationResult.Error($"The directory {commandSettings.Directory} does not exist.");

            return base.Validate(context, commandSettings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] PolicyListCommandSettings commandSettings)
        {
            var directory = commandSettings.Directory;

            if (string.IsNullOrWhiteSpace(directory))
                directory = Environment.CurrentDirectory;

            if (!directory.EndsWith(Path.DirectorySeparatorChar))
                directory += Path.DirectorySeparatorChar;

            var searchOption = commandSettings.Recursive.GetValueOrDefault(false) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var table = new Table();
            table.AddColumn("Policy");
            table.AddColumn("Resource");
            table.AddColumn("Created");
            table.AddColumn("Last Updated");
            foreach (var policy in Directory.EnumerateFiles(directory, "*.policy", searchOption).OrderBy(x => x))
            {
                var fi = new FileInfo(policy);
                var policyName = fi.FullName;
                var resource = _policyCompiler.GetResourceForPolicyFile(policy) ?? "[italic red]<invalid policy>[/]";
                table.AddRow(policyName, resource, fi.CreationTime.ToString("O"), fi.LastWriteTime.ToString("O"));
            }

            if (table.Rows.Count > 0)
                AnsiConsole.Render(table);

            return 0;
        }
    }
}
