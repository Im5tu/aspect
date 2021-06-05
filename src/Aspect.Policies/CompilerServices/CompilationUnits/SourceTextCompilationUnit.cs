namespace Aspect.Policies.CompilerServices.CompilationUnits
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
