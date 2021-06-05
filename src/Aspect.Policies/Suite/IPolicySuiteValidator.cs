namespace Aspect.Policies.Suite
{
    public interface IPolicySuiteValidator
    {
        PolicySuiteValidationResult Validate(PolicySuite policySuite);
    }
}
