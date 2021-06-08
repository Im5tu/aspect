using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Aspect.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal sealed class PolicyBuildCommand : Command<PolicyBuildCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
            [CommandOption("-f|--filename")]
            public string? FileName { get; init; }

            [CommandOption("--no-ui")]
            public bool? NoUi { get; init; }

            [CommandOption("-s|--suite")]
            public bool? Suite { get; init; }
        }

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;
        private readonly IPolicyLoader _policyLoader;

        public PolicyBuildCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicyCompiler policyCompiler,
            IPolicySuiteSerializer policySuiteSerializer,
            IPolicyLoader policyLoader)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
            _policySuiteSerializer = policySuiteSerializer;
            _policyLoader = policyLoader;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            string policy;
            var isSuite = settings.Suite.GetValueOrDefault(false);
            if (isSuite)
            {
                policy = GeneratePolicySuite();
            }
            else
            {
                policy = GeneratePolicy();
            }

            AnsiConsole.Clear();

            if (!string.IsNullOrWhiteSpace(settings.FileName))
            {
                if ((isSuite && !settings.FileName.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                    || (!isSuite && !settings.FileName.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    AnsiConsole.MarkupLine($"[orange1]Warning: [/]The filename provided doesn't match the type of file you meant to generate. Please use '{(isSuite ? FileExtensions.PolicySuiteExtension : FileExtensions.PolicyFileExtension)}' to have it picked up correctly by this CLI.");
                    AnsiConsole.WriteLine();
                }

                using var fs = File.CreateText(settings.FileName);
                fs.Write(policy);
                fs.Flush();
                fs.Close();
            }

            if (!settings.NoUi.GetValueOrDefault(false))
            {
                AnsiConsole.WriteLine(policy);
            }

            return 0;
        }

        private string GeneratePolicy()
        {
            var provider = _cloudProviders[this.PromptOrDefault("Select cloud provider:", _cloudProviders.Keys, "AWS")];
            var resource = this.PromptOrDefault("Select your resource:", provider.GetResources().Keys);
            var statementExpressions = this.MultiSelect("Select your optional statement blocks:", new[] {"include", "exclude"}, false)
                .Concat(new [] {"validate"})
                .ToList();

            var policyBuilder = new StringBuilder($"resource \"{resource}\"");
            policyBuilder.AppendLine();

            foreach (var se in statementExpressions)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[grey]You can view the available properties for the input object at: https://cloudaspect.app/docs/{provider.Name.ToLowerInvariant()}/resources/{resource.ToLowerInvariant()} [/]");
                AnsiConsole.MarkupLine("[grey]You can view the available functions at: https://cloudaspect.app/docs/getting-started/builtin-functions/ [/]");
                AnsiConsole.MarkupLine("[grey]Press enter after you've entered your statement to have it validated. To continue, press enter on an empty expressi[/]");

                var statements = new List<string>();

                string answer;
                var requiresRetry = false;
                do
                {
                    answer = this.Ask($"Enter statement for the {se} block:", true);

                    if (string.IsNullOrWhiteSpace(answer) && !(se == "validate" && statements.Count == 0))
                        break;

                    var sourceText = $@"resource ""{resource}""

validate {{
    {answer}
}}";
                    requiresRetry = !_policyCompiler.IsPolicyValid(new SourceTextCompilationUnit(sourceText), out var context);
                    foreach (var error in context.Errors)
                        AnsiConsole.MarkupLine(error.FormatMessage(false));

                    if (!requiresRetry)
                        statements.Add(answer);

                } while (requiresRetry || !string.IsNullOrWhiteSpace(answer));

                if (statements.Count > 0)
                {
                    policyBuilder.AppendLine($"{se} {{");
                    foreach (var statement in statements)
                        policyBuilder.AppendLine($"    {statement.Trim()}");
                    policyBuilder.AppendLine("}");
                }
            }

            return policyBuilder.ToString();
        }

        private string GeneratePolicySuite()
        {
            var policySuite = new PolicySuite();
            var policyElements = new List<PolicyElement>();
            policySuite.Name = this.Ask("Enter a name for your policy suite:");
            policySuite.Description = this.Ask("Describe your policy suite [grey](optional)[/]:", true).NullIfEmpty();
            policySuite.Policies = policyElements;

            var choices = _cloudProviders.Keys.ToList();
            string provider;
            do
            {
                var element = new PolicyElement();
                element.Type = provider = this.PromptOrDefault("Select a cloud provider to configure:", choices);

                if (provider == "Exit")
                    break;

                var policies = new List<string>();
                element.Regions = this.MultiSelect("Which regions would you like to check?", _cloudProviders[provider].GetAllRegions());
                element.Name = this.Ask("What's the name for this configuration?");
                element.Description = this.Ask("How would you describe this configuration? [grey](optional)[/]", true).NullIfEmpty();
                element.Policies = policies;

                var canExit = false;
                do
                {
                    var policy = this.Ask($"Enter the path or directory to '{FileExtensions.PolicyFileExtension}' file:", canExit);
                    if (canExit && string.IsNullOrWhiteSpace(policy))
                        break;

                    var validation = _policyLoader.ValidateExists(policy);
                    if (validation is null)
                    {
                        canExit = true;
                        policies.Add(policy);
                    }
                    else
                        AnsiConsole.MarkupLine($"[red]{validation.Message}[/]");
                } while (!canExit);

                policyElements.Add(element);

                // after the first loop through we can all the user to exit
                if (choices.Count == _cloudProviders.Keys.Count())
                    choices = choices.Concat(new[] {"Exit"}).ToList();
            } while (provider != "Exit");

            return _policySuiteSerializer.Serialize(policySuite);
        }
    }
}
