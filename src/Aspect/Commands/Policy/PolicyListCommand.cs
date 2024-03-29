﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.CompilerServices;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal sealed class PolicyListCommand : Command<PolicyListCommand.Settings>
    {
        internal class Settings : DirectorySettings
        {
            [Description("Recurses through child directories to find policies.")]
            [CommandOption("-r|--recursive")]
            public bool? Recursive { get; init; }
        }

        private readonly IPolicyCompiler _policyCompiler;
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;

        public PolicyListCommand(IPolicyCompiler policyCompiler, IBuiltInPolicyProvider builtInPolicyProvider)
        {
            _policyCompiler = policyCompiler;
            _builtInPolicyProvider = builtInPolicyProvider;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings commandSettings)
        {
            if (!string.IsNullOrWhiteSpace(commandSettings.Directory) && !commandSettings.Directory.StartsWith("builtin", StringComparison.OrdinalIgnoreCase) && !Directory.Exists(commandSettings.Directory))
                return ValidationResult.Error($"The directory {commandSettings.Directory} does not exist.");

            return base.Validate(context, commandSettings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings commandSettings)
        {
            var directory = commandSettings.Directory;

            if (string.IsNullOrWhiteSpace(directory))
                directory = "builtin";

            if (!directory.StartsWith("builtin", StringComparison.OrdinalIgnoreCase) && !directory.EndsWith(Path.DirectorySeparatorChar))
                directory += Path.DirectorySeparatorChar;

            var rows = directory.StartsWith("builtin", StringComparison.OrdinalIgnoreCase)
                ? GetBuiltInPolicies(directory)
                : GetPoliciesFromDirectory(directory, commandSettings.Recursive.GetValueOrDefault(false) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var table = new Table();
            table.AddColumn("Policy");
            table.AddColumn("Resource");
            table.AddColumn("Created");
            table.AddColumn("Last Updated");
            foreach (var row in rows.OrderBy(x => x.policy))
                table.AddRow(row.policy, row.resource, row.creationDate, row.lastUpdated);

            if (table.Rows.Count == 0)
                table.AddRow("", "", "", "");

            AnsiConsole.Render(table);

            return 0;
        }

        private IEnumerable<(string policy, string resource, string creationDate, string lastUpdated)> GetBuiltInPolicies(string partial)
        {
            foreach (var resource in _builtInPolicyProvider.GetAllResources())
            {
                if (!resource.Name.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                    continue;

                if (resource.Name.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    var r = _policyCompiler.GetResourceForPolicy(resource) ?? "[italic red]Invalid Resource[/]";
                    yield return (resource.Name, r, "", "");
                }
                else
                {
                    yield return (resource.Name, "Various", "", "");
                }
            }
        }

        private IEnumerable<(string policy, string resource, string creationDate, string lastUpdated)> GetPoliciesFromDirectory(string directory, SearchOption searchOption)
        {
            var rows = new List<(string policy, string resource, string creationDate, string lastUpdated)>();
            foreach (var policy in Directory.EnumerateFiles(directory, $"*{FileExtensions.PolicyFileExtension}", searchOption))
            {
                var fi = new FileInfo(policy);
                var policyName = fi.FullName;
                var resource = _policyCompiler.GetResourceForPolicy(policy) ?? "[italic red]Invalid Resource[/]";
                rows.Add((policyName, resource, fi.CreationTime.ToString("O"), fi.LastWriteTime.ToString("O")));
            }

            foreach (var policy in Directory.EnumerateFiles(directory, $"*{FileExtensions.PolicySuiteExtension}", searchOption))
            {
                var fi = new FileInfo(policy);
                var policyName = fi.FullName;
                // TODO :: could we list the resources by extracting them out? This could be a lot of IO though...?
                rows.Add((policyName, "Various", fi.CreationTime.ToString("O"), fi.LastWriteTime.ToString("O")));
            }

            return rows;
        }
    }
}
