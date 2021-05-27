using System;
using System.Collections.Generic;
using System.IO;
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
            Console.WriteLine(FormatMessage("Error", code, line, position));
            _errors.Add(new CompilationError(code, line.GetValueOrDefault(), position.GetValueOrDefault()));
        }

        internal void RaiseWarning(string code, SyntaxToken token)
            => RaiseWarning(code, token.LineNumberRaw, token.PositionRaw);

        internal void RaiseWarning(string code, int line, int position)
        {
            Console.WriteLine(FormatMessage("Warning", code, line, position));
            _warnings.Add(new CompilationWarning(code, line, position));
        }

        private string FormatMessage(string type, string code, int? line = null, int? position = null)
        {
            var msg = $"{type}: {code} - {GetMessageForCode(code)}.";

            if (line.HasValue)
                msg += $" Line: {line + 1} Position: {position + 1}";

            if (Source is FileCompilationUnit fcu)
                msg += $" (File: {new FileInfo(fcu.FileName).Name})";

            return msg;
        }
        private static string GetMessageForCode(string code)
        {
            // TODO :: TASK :: Proper error messages
            return code switch
            {
                // Lexer Errors
                "CA-LEX-001" => "Negation & assignment are not currently supported",
                "CA-LEX-002" => "Escape sequence in quoted identifier is invalid (supported: \\\")",
                "CA-LEX-003" => "Missing ending quotation",
                "CA-LEX-004" => "Empty policy provided",
                "CA-LEX-005" => "Symbol not supported",
                // Parser Errors
                "CA-PAR-001" => "Invalid syntax, nested block.",
                "CA-PAR-002" => "Invalid syntax, block closed before it's opened",
                "CA-PAR-003" => "Invalid token",
                "CA-PAR-004" => "No validation statements in policy",
                "CA-PAR-005" => "Invalid top level identifier",
                "CA-PAR-006" => "Resource not specified in policy",
                "CA-PAR-007" => "Resource policy applies to is not present",
                "CA-PAR-008" => "Type mismatch in expression",
                "CA-PAR-009" => "Resource that policy specifies cannot be found",
                "CA-PAR-010" => "No property specified on 'input'",
                "CA-PAR-011" => "Statements must be against a function or the value 'input'",
                "CA-PAR-012" => "Property does not exist on type",
                "CA-PAR-013" => "Invalid syntax. Collection access must be input.Something[_] / input.Something[*] / input.Something[<int>]",
                "CA-PAR-014" => "Invalid function name",
                "CA-PAR-015" => "Operators cannot be the last element in a statement",
                _ => "An Unknown error occurred whilst compiling the policy"
            };
        }
    }
}
