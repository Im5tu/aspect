using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Policies.Suite;
using Aspect.Services;

namespace Aspect.Commands.Run
{
    internal sealed class RunPolicyRunnerCommandStage : CommandStage<RunCommand.Settings>
    {
        private readonly IPolicySuiteRunner _policySuiteRunner;
        private readonly IFormatterFactory _formatterFactory;

        public RunPolicyRunnerCommandStage(IPolicySuiteRunner policySuiteRunner, IFormatterFactory formatterFactory)
        {
            _policySuiteRunner = policySuiteRunner;
            _formatterFactory = formatterFactory;
        }

        internal override async Task<int?> InvokeAsync(IExecutionData data, RunCommand.Settings settings)
        {
            var policy = data.Get<PolicySuite>(nameof(PolicySuite));
            var results = await _policySuiteRunner.RunPoliciesAsync(policy, default);

            var formatter = _formatterFactory.GetFormatterFor(settings.Formatter.GetValueOrDefault(FormatterType.Json));
            var formattedResult = formatter.Format(results);

            await File.WriteAllTextAsync(settings.OutputFile!, formattedResult);

            if (results.Errors?.Any() ?? false)
                 return 2;

            if (results.FailedResources?.Any() ?? false)
                return -1;

            return 0;
        }

        private class Result
        {
            public List<string>? Errors { get; set; }
            public List<PolicySuiteRunResult.FailedResource>? FailedResources { get; set; }
        }
    }
}
