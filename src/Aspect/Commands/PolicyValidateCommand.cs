using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyValidateCommand : Command<PolicyValidateCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;

        public PolicyValidateCommand(IPolicyCompiler policyCompiler, IPolicySuiteValidator policySuiteValidator, IPolicySuiteSerializer policySuiteSerializer, IBuiltInPolicyProvider builtInPolicyProvider)
        {
            _policyCompiler = policyCompiler;
            _policySuiteValidator = policySuiteValidator;
            _policySuiteSerializer = policySuiteSerializer;
            _builtInPolicyProvider = builtInPolicyProvider;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] PolicyValidateCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Source))
                return ValidationResult.Error("Please specify a filename, directory name or builtin");

            if (!settings.Source.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
            {
                var isDirectory = File.GetAttributes(settings.Source!).HasFlag(FileAttributes.Directory);
                if (isDirectory)
                {
                    if (!Directory.Exists(settings.Source))
                        return ValidationResult.Error("Directory cannot be found");
                }
                else if (!File.Exists(settings.Source))
                    return ValidationResult.Error("File cannot be found");
            }

            return base.Validate(context, settings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] PolicyValidateCommandSettings settings)
        {
            var validatedFiles = new List<(string source, bool isValid, List<string> errors)>();

            if (settings.Source!.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var resource in _builtInPolicyProvider.GetAllResources())
                {
                    if (!resource.Name.StartsWith(settings.Source, StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (resource.Name.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        var isValid = _policyCompiler.IsPolicyValid(resource, out var cntx);
                        validatedFiles.Add((resource.Name, isValid, cntx.Errors.Select(x => x.FormatMessage()).ToList()));
                    }
                    else
                    {
                        // TODO : TASK : Can we create a more generic policy loader? Would it be beneficial?
                        var result = _policySuiteValidator.Validate(_policySuiteSerializer.Deserialize(resource.GetAllText()));
                        validatedFiles.Add((resource.Name, result.IsValid, result.Errors.ToList()));
                    }
                }
            }
            else
            {
                var isDirectory = File.GetAttributes(settings.Source!).HasFlag(FileAttributes.Directory);
                var policyFiles = new List<FileCompilationUnit>();
                var policySuites = new List<string>();

                if (isDirectory)
                {
                    var directory = settings.Source!;
                    if (!directory.EndsWith(Path.DirectorySeparatorChar))
                        directory += Path.DirectorySeparatorChar;

                    var searchOption = settings.Recursive.GetValueOrDefault(false) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    policyFiles.AddRange(Directory.EnumerateFiles(directory, $"*{FileExtensions.PolicyFileExtension}", searchOption).Select(x => new FileCompilationUnit(x)));
                    policySuites.AddRange(Directory.EnumerateFiles(directory, $"*{FileExtensions.PolicySuiteExtension}", searchOption));
                }
                else
                {
                    policyFiles.Add(new FileCompilationUnit(settings.Source!));
                }

                foreach (var file in policyFiles)
                {
                    var isValid = _policyCompiler.IsPolicyValid(file, out var cntx);
                    validatedFiles.Add((file.FileName, isValid, cntx.Errors.Select(x => x.FormatMessage()).ToList()));
                }

                foreach (var suite in policySuites)
                {
                    var result = _policySuiteValidator.Validate(_policySuiteSerializer.Deserialize(File.ReadAllText(suite)));
                    validatedFiles.Add((suite, result.IsValid, result.Errors.ToList()));
                }
            }

            var failedOnly = settings.FailedOnly.GetValueOrDefault(false);
            var table = new Table();
            table.AddColumns("Source", "Is Valid?", "Errors");

            foreach (var (source, isValid, errors) in validatedFiles.OrderBy(x => x.source))
            {
                if (failedOnly && !isValid)
                    table.AddRow(source, "[red]Invalid[/]", string.Join(Environment.NewLine, errors.Select(x => $"- {x}")));
                else if (!failedOnly)
                    table.AddRow(source, isValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, errors.Select(x => $"- {x}")));
            }

            AnsiConsole.Render(table);

            return validatedFiles.Any(x => x.errors.Count > 0) ? 1 : 0;
        }
    }
}
