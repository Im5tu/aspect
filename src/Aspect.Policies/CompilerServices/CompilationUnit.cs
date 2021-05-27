namespace Aspect.Policies.CompilerServices
{
    /// <summary>
    ///     Represents a source of data for compilation
    /// </summary>
    public abstract class CompilationUnit
    {
        /// <summary>
        ///     Gets the policy text from the underlying source
        /// </summary>
        public abstract string GetAllText();
    }
}
