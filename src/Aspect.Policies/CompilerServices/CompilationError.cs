namespace Aspect.Policies.CompilerServices
{
    /// <summary>
    ///     Represents a compilation error
    /// </summary>
    public record CompilationError(string Code, int Line, int Position);
}
