using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Formatters;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Aspect.Policies.Suite;
using Aspect.Runners;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class RunCommand : AsyncCommand<RunCommandSettings>
    {
        private readonly IPolicySuiteRunner _policySuiteRunner;
        private readonly IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;
        private bool _isDirectory = false;
        private bool _isPolicySuite = false;
        private bool _isBuiltIn = false;

        public RunCommand(IPolicySuiteRunner policySuiteRunner,
            IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicySuiteValidator policySuiteValidator,
            IBuiltInPolicyProvider builtInPolicyProvider,
            IPolicySuiteSerializer policySuiteSerializer)
        {
            _policySuiteRunner = policySuiteRunner;
            _cloudProviders = cloudProviders;
            _policySuiteValidator = policySuiteValidator;
            _builtInPolicyProvider = builtInPolicyProvider;
            _policySuiteSerializer = policySuiteSerializer;
        }

        public override ValidationResult Validate(CommandContext context, RunCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileOrDirectory))
                return ValidationResult.Error("Please specify a path of a policy file, policy suite or directory.");

            if (settings.FileOrDirectory.StartsWith("builtin\\", StringComparison.OrdinalIgnoreCase))
            {
                _isBuiltIn = true;
            }
            else
            {
                _isDirectory = File.GetAttributes(settings.FileOrDirectory).HasFlag(FileAttributes.Directory);

                if (_isDirectory && !Directory.Exists(settings.FileOrDirectory))
                    return ValidationResult.Error($"Specified directory '{settings.FileOrDirectory}' does not exist.");
                if (!File.Exists(settings.FileOrDirectory))
                    return ValidationResult.Error($"Specified file '{settings.FileOrDirectory}' does not exist.");

                var fi = new FileInfo(settings.FileOrDirectory);

                if (fi.Extension.Equals(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                    _isPolicySuite = true;
                else if (!fi.Extension.Equals(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                    return ValidationResult.Error($"Filename must end with either '{FileExtensions.PolicyFileExtension}' or '{FileExtensions.PolicySuiteExtension}'.");
            }

            return base.Validate(context, settings);
        }

        public override async Task<int> ExecuteAsync(CommandContext context, RunCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileOrDirectory))
                return -1;

            var policy = LoadPolicySuite(settings.FileOrDirectory);
            var validationResult = _policySuiteValidator.Validate(policy);
            if (!validationResult.IsValid)
            {
                var table = new Table();
                table.AddColumns("Policy", "IsValid", "Errors");
                table.AddRow(settings.FileOrDirectory, validationResult.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, validationResult.Errors.Select(x => $"- {x}")));
                AnsiConsole.Render(table);
                return 1;
            }

            var results = (await _policySuiteRunner.RunPoliciesAsync(policy, default)).ToList();
            var formattedResult = new Result
            {
                Errors = results.Where(x => x.Error is not null).Select(x => x.Error!).ToList(),
                FailedResources = results.Where(x => x.FailedResources is not null).SelectMany(x => x.FailedResources!).ToList()
            };

            await new JsonFormatter().FormatAsync(formattedResult);

            return 0;
        }

        private PolicySuite LoadPolicySuite(string name)
        {
            PolicySuite result;

            if (_isBuiltIn || _isPolicySuite)
                result = LoadPolicySuiteFromName(name);
            else
            {
                result = new PolicySuite
                {
                    Policies = _cloudProviders.Select(x => new PolicyElement { Type = x.Key, Name = x.Key, Regions = x.Value.GetAllRegions(), Policies = new [] { name }}).ToList()
                };
            }

            return result;
        }

        private PolicySuite LoadPolicySuiteFromName(string name)
        {
            if (!name.EndsWith(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid policy suite");

            if (name.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
            {
                if (_builtInPolicyProvider.TryGetPolicySuite(name, out var policySuite))
                    return policySuite;
            }
            else
                return _policySuiteSerializer.Deserialize(File.ReadAllText(name));

            throw new Exception("Policy not found");
        }

        private class Result
        {
            public IEnumerable<string>? Errors { get; set; }
            public IEnumerable<PolicySuiteRunResult.FailedResource>? FailedResources { get; set; }
        }
    }
}
