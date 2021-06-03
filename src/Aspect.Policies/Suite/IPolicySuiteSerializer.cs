namespace Aspect.Policies.Suite
{
    internal interface IPolicySuiteSerializer
    {
        string Serialize(PolicySuite suite);

        PolicySuite Deserialize(string policySuite);
    }
}
