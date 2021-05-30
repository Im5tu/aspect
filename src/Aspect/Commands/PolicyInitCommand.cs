using System.Diagnostics.CodeAnalysis;
using System.IO;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyInitCommand : Command<PolicyInitCommandSettings>
    {
        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] PolicyInitCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileName))
                return ValidationResult.Error("Please specify a file name.");

            if (File.Exists(settings.FileName))
                return ValidationResult.Error($"The file {settings.FileName} already exists.");

            return base.Validate(context, settings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] PolicyInitCommandSettings settings)
        {
            using (var file = File.CreateText(settings.FileName!))
            {
                file.Write(@"resource ""CHANGEME""

validate {
    input.Property == ""something""
}");
                file.Flush();
                file.Close();
            }

            return 0;
        }
    }
}
