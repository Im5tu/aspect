namespace Aspect.Policies.CompilerServices.CompilationUnits
{
    public class BuiltInResourceCompilationUnit : CompilationUnit
    {
        public string Name { get; }
        public string Policy { get; }

        public BuiltInResourceCompilationUnit(string name, string policy)
        {
            Name = name;
            Policy = policy;
        }

        public override string GetAllText() => Policy;
    }
}
