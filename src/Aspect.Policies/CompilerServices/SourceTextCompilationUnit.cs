namespace Aspect.Policies.CompilerServices
{
    internal class SourceTextCompilationUnit : CompilationUnit
    {
        public string SourceText { get; }

        public SourceTextCompilationUnit(string sourceText)
        {
            SourceText = sourceText;
        }

        public override string GetAllText() => SourceText;
    }
}
