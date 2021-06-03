using System.Collections.Generic;

namespace Aspect.Policies.Suite
{
    public class PolicySuiteValidationResult
    {
        private readonly List<string> _errors = new();

        public bool IsValid => _errors.Count == 0;
        public IEnumerable<string> Errors => _errors;

        internal void AddError(string error) => _errors.Add(error);

        internal static PolicySuiteValidationResult Error(string error)
        {
            var result = new PolicySuiteValidationResult();
            result.AddError(error);
            return result;
        }
    }
}
