using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Aspect.Extensions;
using Aspect.Policies.CompilerServices;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyValidateCommand : Command<PolicyValidateCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;

        public PolicyValidateCommand(IPolicyCompiler policyCompiler)
        {
            _policyCompiler = policyCompiler;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] PolicyValidateCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileOrDirectory))
                return ValidationResult.Error("Please specify a file or directory name.");

            var isDirectory = File.GetAttributes(settings.FileOrDirectory!).HasFlag(FileAttributes.Directory);
            if (isDirectory)
            {
                if (!Directory.Exists(settings.FileOrDirectory))
                    return ValidationResult.Error("Directory cannot be found");
            }
            else if (!File.Exists(settings.FileOrDirectory))
                return ValidationResult.Error("File cannot be found");

            return base.Validate(context, settings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] PolicyValidateCommandSettings settings)
        {
            var isDirectory = File.GetAttributes(settings.FileOrDirectory!).HasFlag(FileAttributes.Directory);
            var policyFiles = new List<string>();

            if (isDirectory)
            {
                var directory = settings.FileOrDirectory!;
                if (!directory.EndsWith("\\", StringComparison.Ordinal))
                    directory += "\\";

                var searchOption = settings.Recursive.GetValueOrDefault(false) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                policyFiles.AddRange(Directory.EnumerateFiles(directory, "*.policy", searchOption));
            }
            else
            {
                policyFiles.Add(settings.FileOrDirectory!);
            }

            var failedOnly = settings.FailedOnly.GetValueOrDefault(false);
            var results = new List<CompilationContext>();
            var directoryHasErrors = false;
            foreach (var policy in policyFiles.OrderBy(x => x))
            {
                var isValid = _policyCompiler.IsPolicyFileValid(policy, out var cntx);
                results.Add(cntx);
                directoryHasErrors |= !isValid;
            }

            results.WriteCompilationResultToConsole(failedOnly);

            return directoryHasErrors ? 1 : 0;
        }
    }
}
