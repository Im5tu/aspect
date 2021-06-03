using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aspect.Abstractions;
using Aspect.Extensions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class PolicyInitCommand : Command<PolicyInitCommandSettings>
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public PolicyInitCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

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
            string resource;
            if (string.IsNullOrWhiteSpace(settings.Resource))
            {
                var provider = _cloudProviders[ConsoleExtensions.PromptOrDefault("Select cloud provider:", _cloudProviders.Keys, "AWS")];
                var resources = provider.GetResources();
                resource = ConsoleExtensions.PromptOrDefault("Select resource:", resources.Keys);
            }
            else
            {
                resource = settings.Resource;
            }

            using (var file = File.CreateText(settings.FileName!))
            {
                file.Write($@"resource ""{resource}""

validate {{
    input.Property == ""something""
}}");
                file.Flush();
                file.Close();
            }

            return 0;
        }
    }
}
