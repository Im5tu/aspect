using System.Diagnostics.CodeAnalysis;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.Suite;
using Aspect.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal sealed class PolicyViewCommand : Command<PolicyViewCommand.Settings>
    {
        private readonly IPolicyLoader _policyLoader;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        internal sealed class Settings : FileSettings
        {
        }

        public PolicyViewCommand(IPolicyLoader policyLoader, IPolicySuiteSerializer policySuiteSerializer)
        {
            _policyLoader = policyLoader;
            _policySuiteSerializer = policySuiteSerializer;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            return _policyLoader.ValidateExists(settings.FileName) ?? base.Validate(context, settings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            var policy = _policyLoader.LoadPolicy(settings.FileName!);
            if (policy is { })
            {
                AnsiConsole.WriteLine(policy.GetAllText());
                return 0;
            }

            var suite = _policyLoader.LoadPolicySuite(settings.FileName!);
            if (suite is { })
            {
                AnsiConsole.WriteLine(_policySuiteSerializer.Serialize(suite));
                return 0;
            }

            return 1;
        }
    }
}
