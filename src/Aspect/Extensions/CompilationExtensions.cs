using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Spectre.Console;

namespace Aspect.Extensions
{
    internal static class CompilationExtensions
    {
        internal static string FormatMessage(this CompilationError error)
        {
            return $"[red]{error.Code} - {GetMessageForCode(error.Code)} - Line {error.Line} Position: {error.Position}[/]";
        }

        internal static string FormatMessage(this CompilationWarning warning)
        {
            return $"[orange1]{warning.Code} - {GetMessageForCode(warning.Code)} - Line {warning.Line} Position: {warning.Position}[/]";
        }

        internal static string GetPolicyName(this CompilationUnit source)
        {
            var policy = "inline";
            if (source is FileCompilationUnit fcu)
                policy = new FileInfo(fcu.FileName).FullName;
            else if (source is BuiltInResourceCompilationUnit bircu)
                policy = bircu.Name;

            return policy;
        }

        internal static string GetMessageForCode(string code)
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

        internal static void WriteCompilationResultToConsole(this IEnumerable<CompilationContext> contexts, bool failedOnly = false)
        {
            var table = new Table();
            table.AddColumn("Policy");
            table.AddColumn("IsValid?");
            table.AddColumn("Errors");
            table.AddColumn("Warnings");

            foreach (var cntx in contexts)
            {
                var errors = string.Join(Environment.NewLine, cntx.Errors.Select(x => x.FormatMessage()));
                var warnings = string.Join(Environment.NewLine, cntx.Warnings.Select(x => x.FormatMessage()));
                var policy = cntx.Source.GetPolicyName();

                if (failedOnly)
                {
                    if (!cntx.IsValid)
                        table.AddRow(policy, "[red]Invalid[/]", errors, warnings);
                }
                else
                    table.AddRow(policy, cntx.IsValid ? "[green]Valid[/]" : "[red]Invalid[/]", errors, warnings);
            }

            if (table.Rows.Count > 0)
                AnsiConsole.Render(table);
        }

        internal static void WriteCompilationResultToConsole(this CompilationContext context) => new[] {context}.WriteCompilationResultToConsole();
    }
}
