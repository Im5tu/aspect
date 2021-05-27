namespace Aspect.Policies.CompilerServices
{
    /// <summary>
    ///     Represents a compilation warning
    /// </summary>
    public record CompilationWarning(string Code, int Line, int Position);
}
