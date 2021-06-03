namespace Aspect.Policies
{
    public interface IPolicySuiteValidator
    {
        PolicySuiteValidationResult Validate(PolicySuite policySuite);
    }
}
