using System.Collections.Generic;
using Aspect.Policies.CompilerServices.SyntaxTokens;

namespace Aspect.Policies.CompilerServices
{
    /// <summary>
    ///     Represents a context under which a policy document/statement is compiled and checked for correctness
    /// </summary>
    public class CompilationContext
    {
        private readonly List<CompilationError> _errors = new();
        private readonly List<CompilationWarning> _warnings = new();

        /// <summary>
        ///     The source of the policy
        /// </summary>
        public CompilationUnit Source { get; }
        /// <summary>
        ///     A collection of compilation errors
        /// </summary>
        public IEnumerable<CompilationError> Errors => _errors;
        /// <summary>
        ///     A collection of compilation warnings
        /// </summary>
        public IEnumerable<CompilationWarning> Warnings => _warnings;

        /// <summary>
        ///     Determines whether there are errors after the compiler has run
        /// </summary>
        public bool IsValid => _errors.Count == 0;

        /// <param name="source">The source to use for the compilation</param>
        public CompilationContext(CompilationUnit source)
        {
            Source = source;
        }

        internal void RaiseError(string code, SyntaxToken token)
            => RaiseError(code, token.LineNumberRaw, token.PositionRaw);

        internal void RaiseError(string code, int? line = null, int? position = null)
        {
            _errors.Add(new CompilationError(code, line.GetValueOrDefault(), position.GetValueOrDefault()));
        }

        internal void RaiseWarning(string code, SyntaxToken token)
            => RaiseWarning(code, token.LineNumberRaw, token.PositionRaw);

        internal void RaiseWarning(string code, int line, int position)
        {
            _warnings.Add(new CompilationWarning(code, line, position));
        }
    }
}
