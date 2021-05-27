namespace Aspect.Policies.CompilerServices.Generator
{
    /// <summary>
    ///     The result of a policy evaulation
    /// </summary>
    public enum ResourcePolicyExecution
    {
        /// <summary>
        ///     The input object was null
        /// </summary>
        Null,
        /// <summary>
        ///     The policy was not evaluated because the input object was not of the specified type
        /// </summary>
        SkippedByType,
        /// <summary>
        ///     The policy was not evaluated because the input object did not meet the requirements of the policies include/exclude blocks
        /// </summary>
        SkippedByPolicy,
        /// <summary>
        ///     The policy was successfully validated against the input object
        /// </summary>
        Passed,
        /// <summary>
        ///     The input object did not validate against the policy document
        /// </summary>
        Failed
    }
}
