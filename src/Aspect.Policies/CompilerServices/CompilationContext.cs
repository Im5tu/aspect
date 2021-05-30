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
        public IReadOnlyCollection<CompilationError> Errors => _errors;
        /// <summary>
        ///     A collection of compilation warnings
        /// </summary>
        public IReadOnlyCollection<CompilationWarning> Warnings => _warnings;

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
