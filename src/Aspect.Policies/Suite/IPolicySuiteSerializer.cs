namespace Aspect.Policies.Suite
{
    internal interface IPolicySuiteSerializer
    {
        PolicySuite Deserialize(string policySuite);
    }
}