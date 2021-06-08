using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Policies;
using Aspect.Policies.Suite;
using Aspect.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal class RunCommand : AsyncCommand<RunCommand.RunCommandSettings>
    {
        internal class RunCommandSettings : FileOrDirectorySettings
        {
            [CommandOption("--format")]
            public FormatterType? Formatter { get; init; }
        }

        private readonly IPolicySuiteRunner _policySuiteRunner;
        private readonly IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IPolicyLoader _policyLoader;
        private readonly IFormatterFactory _formatterFactory;

        public RunCommand(IPolicySuiteRunner policySuiteRunner,
            IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicySuiteValidator policySuiteValidator,
            IPolicyLoader policyLoader,
            IFormatterFactory formatterFactory)
        {
            _policySuiteRunner = policySuiteRunner;
            _cloudProviders = cloudProviders;
            _policySuiteValidator = policySuiteValidator;
            _policyLoader = policyLoader;
            _formatterFactory = formatterFactory;
        }

        public override ValidationResult Validate(CommandContext context, RunCommandSettings settings)
        {
            var source = settings.Source;

            if (string.IsNullOrWhiteSpace(source))
                source = Environment.CurrentDirectory;

            return _policyLoader.ValidateExists(source) ?? base.Validate(context, settings);
        }

        public override async Task<int> ExecuteAsync(CommandContext context, RunCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Source))
                return -1;

            var policy = LoadPolicySuite(settings.Source);
            var validationResult = _policySuiteValidator.Validate(policy);
            if (!validationResult.IsValid)
            {
                var table = new Table();
                table.AddColumns("Policy", "IsValid", "Errors");
                table.AddRow(settings.Source, validationResult.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, validationResult.Errors.Select(x => $"- {x}")));
                AnsiConsole.Render(table);
                return 1;
            }

            var results = (await _policySuiteRunner.RunPoliciesAsync(policy, default)).ToList();
            var aggregatedResult = new Result
            {
                Errors = results.Where(x => x.Error is not null).Select(x => x.Error!).ToList(),
                FailedResources = results.Where(x => x.FailedResources is not null).SelectMany(x => x.FailedResources!).ToList()
            };

            var formatter = _formatterFactory.GetFormatterFor(settings.Formatter.GetValueOrDefault(FormatterType.Json));
            Console.WriteLine(formatter.Format(aggregatedResult));

            if (aggregatedResult.Errors.Count > 0)
                return 2;

            if (aggregatedResult.FailedResources.Count > 0)
                return -1;

            return 0;
        }

        private PolicySuite LoadPolicySuite(string name)
        {
            PolicySuite result;

            if (name.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
            {
                result = _policyLoader.LoadPolicySuite(name) ?? throw new Exception("Policy suite not found.");
            }
            else
            {
                result = new PolicySuite
                {
                    Name = "Policy: " + name,
                    Policies = _cloudProviders.Select(x => new PolicyElement { Type = x.Key, Name = x.Key, Regions = x.Value.GetDefaultRegions(), Policies = new [] { name }}).ToList()
                };
            }

            return result;
        }

        private class Result
        {
            public List<string>? Errors { get; set; }
            public List<PolicySuiteRunResult.FailedResource>? FailedResources { get; set; }
        }
    }
}
