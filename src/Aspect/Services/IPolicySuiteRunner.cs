using System.Threading;
using System.Threading.Tasks;
using Aspect.Policies.Suite;

namespace Aspect.Services
{
    internal interface IPolicySuiteRunner
    {
        Task<PolicySuiteRunResult> RunPoliciesAsync(PolicySuite suite, CancellationToken cancellationToken = default);
    }
}
