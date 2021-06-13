using System;
using System.Collections.Generic;
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

        private class Result
        {
            public List<string>? Errors { get; set; }
            public List<PolicySuiteRunResult.FailedResource>? FailedResources { get; set; }
        }
    }
}