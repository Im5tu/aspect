using System.IO;

namespace Aspect.Policies.CompilerServices.CompilationUnits
{
    internal class FileCompilationUnit : CompilationUnit
    {
        public string FileName { get; }

        public FileCompilationUnit(string fileName)
        {
            FileName = fileName;
        }

        public override string GetAllText() => File.ReadAllText(FileName);
    }
}
