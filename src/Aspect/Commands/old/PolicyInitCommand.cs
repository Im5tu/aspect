using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies.Suite;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands.old
{
    internal class PolicyInitCommand : Command<PolicyInitCommand.Settings>
    {
        internal class Settings : FileSettings
        {
            [Description("The resource to build the policy for")]
            [CommandOption("-r|--resource")]
            public string? Resource { get; init; }

            [Description("Initialize a policy suite instead of a policy")]
            [CommandOption("-s|--suite")]
            public bool? InitializeSuite { get; init; }

            [Description("Display the generated policy in the console")]
            [CommandOption("-d|--display")]
            public bool? Display { get; init; }
        }

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IPolicySuiteSerializer _policySuiteSerializer;

        public PolicyInitCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IPolicySuiteSerializer policySuiteSerializer)
        {
            _cloudProviders = cloudProviders;
            _policySuiteSerializer = policySuiteSerializer;
        }

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            if (!string.IsNullOrWhiteSpace(settings.FileName) && File.Exists(settings.FileName))
                return ValidationResult.Error($"The file {settings.FileName} already exists.");

            return base.Validate(context, settings);
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            string policy;
            if (settings.InitializeSuite.GetValueOrDefault(false))
                policy = InitializePolicySuite();
            else
                policy = InitializePolicyFile(settings.Resource);

            if (settings.Display.GetValueOrDefault(false))
                AnsiConsole.WriteLine(policy);

            if (!string.IsNullOrWhiteSpace(settings.FileName))
            {
                using var file = File.CreateText(settings.FileName);
                file.Write(policy);
                file.Flush();
                file.Close();
            }

            return 0;
        }

        private string InitializePolicySuite()
        {
            return _policySuiteSerializer.Serialize(new PolicySuite
            {
                Name = "My Best Practises",
                Description = "Describe what the policy suite does",
                Policies = new []
                {
                    new PolicyElement
                    {
                        Name = "AWS Best Practises",
                        Description = "Describing this section",
                        Type = "AWS",
                        Regions = new [] {"eu-west-1"},
                        Policies = new [] { "D:\\Policies\\MyPolicy.policy" }
                    },
                    new PolicyElement
                    {
                        Name = "Azure Best Practises",
                        Description = "Describing this section",
                        Type = "Azure",
                        Regions = new [] {"uk-south"},
                        Policies = new [] { "D:\\Policies\\MyPolicy.policy" }
                    }
                }
            });
        }

        private string InitializePolicyFile(string? resource)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                var provider = _cloudProviders[this.PromptOrDefault("Select cloud provider:", _cloudProviders.Keys, "AWS")];
                var resources = provider.GetResources();
                resource = this.PromptOrDefault("Select resource:", resources.Keys);
            }

            return $@"resource ""{resource}""

validate {{
    # Enter one or more statements like the following that should be validated, eg: input.Property == ""something""
}}";
        }
    }
}
