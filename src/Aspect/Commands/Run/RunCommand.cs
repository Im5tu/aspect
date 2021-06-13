using System;
using System.Collections.Generic;
using Aspect.Abstractions;
using Aspect.Policies.Suite;
using Aspect.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.Run
{
    internal sealed class RunCommand : ExecutableCommand<RunCommand.Settings>
    {
        private readonly IPolicyLoader _policyLoader;
        private readonly IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IPolicySuiteRunner _policySuitRunner;
        private readonly IFormatterFactory _formatterFactory;

        internal class Settings : FileOrDirectorySettings
        {
            [CommandOption("--format")]
            public FormatterType? Formatter { get; init; }
        }

        public RunCommand(IPolicyLoader policyLoader,
            IReadOnlyDictionary<string, ICloudProvider> cloudProviders,
            IPolicySuiteValidator policySuiteValidator,
            IPolicySuiteRunner policySuitRunner,
            IFormatterFactory formatterFactory)
        {
            _policyLoader = policyLoader;
            _cloudProviders = cloudProviders;
            _policySuiteValidator = policySuiteValidator;
            _policySuitRunner = policySuitRunner;
            _formatterFactory = formatterFactory;
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            var source = settings.Source;

            if (string.IsNullOrWhiteSpace(source))
                source = Environment.CurrentDirectory;

            return _policyLoader.ValidateExists(source) ?? base.Validate(context, settings);
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages(Settings commandSettings)
        {
            yield return new RunPolicyLoaderCommandStage(_policyLoader, _cloudProviders);
            yield return new RunPolicyValidatorCommandStage(_policySuiteValidator);
            yield return new RunPolicyRunnerCommandStage(_policySuitRunner, _formatterFactory);
        }
    }
}
