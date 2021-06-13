using System;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies.Suite;
using Spectre.Console;

namespace Aspect.Commands.Run
{
    internal sealed class RunPolicyValidatorCommandStage : CommandStage<RunCommand.Settings>
    {
        private readonly IPolicySuiteValidator _policySuiteValidator;

        public RunPolicyValidatorCommandStage(IPolicySuiteValidator policySuiteValidator)
        {
            _policySuiteValidator = policySuiteValidator;
        }

        internal override Task<int?> InvokeAsync(IExecutionData data, RunCommand.Settings settings)
        {
            var policy = data.Get<PolicySuite>(nameof(PolicySuite));
            var validationResult = _policySuiteValidator.Validate(policy);
            if (!validationResult.IsValid)
            {
                var table = new Table();
                table.AddColumns("Policy", "IsValid", "Errors");
                table.AddRow(settings.Source!, validationResult.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", string.Join(Environment.NewLine, validationResult.Errors.Select(x => $"- {x}")));
                AnsiConsole.Render(table);
                return Task.FromResult<int?>(2);
            }

            return Task.FromResult<int?>(null);
        }
    }
}