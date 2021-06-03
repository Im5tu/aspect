using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Policies;

namespace Aspect.Runners
{
    internal interface IPolicySuiteRunner
    {
        Task<IEnumerable<PolicySuiteRunResult>> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default);
    }
}
