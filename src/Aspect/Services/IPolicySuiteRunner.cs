using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aspect.Policies.Suite;

namespace Aspect.Services
{
    internal interface IPolicySuiteRunner
    {
        Task<IEnumerable<PolicySuiteRunResult>> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default);
    }
}
