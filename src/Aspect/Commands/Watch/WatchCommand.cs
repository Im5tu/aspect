using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.Suite;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.Watch
{
    internal class WatchCommand : ExecutableCommand<WatchCommand.Settings>
    {
        internal class Settings : DirectorySettings
        {
            [CommandOption("--delay")]
            [Description("The period of time in milliseconds that needs to elapse between changes to a file.")]
            public int? DelayInterval { get; init; }
        }

        private readonly IPolicyCompiler _policyCompiler;
        private readonly IPolicySuiteValidator _policySuiteValidator;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public WatchCommand(IPolicyCompiler policyCompiler,
            IPolicySuiteValidator policySuiteValidator,
            IPolicySuiteSerializer policySuiteSerializer)
        {
            _policyCompiler = policyCompiler;
            _policySuiteValidator = policySuiteValidator;
            _policySuiteSerializer = policySuiteSerializer;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Directory))
                return ValidationResult.Error("Please specify a directory to watch");

            return base.Validate(context, settings);
        }

        protected override IEnumerable<CommandStage<Settings>> GetCommandStages(Settings commandSettings)
        {
            yield return new WatchCommandStage(_policyCompiler, _policySuiteValidator, _policySuiteSerializer);
        }
    }
}
