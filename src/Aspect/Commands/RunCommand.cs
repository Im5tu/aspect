using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Policies;
using Aspect.Runners;
using Spectre.Console;
using Spectre.Console.Cli;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Aspect.Commands
{
    internal class RunCommand : AsyncCommand<RunCommandSettings>
    {
        private readonly IPolicySuiteRunner _policySuiteRunner;
        private bool _isDirectory = false;
        private bool _isPolicySuite = false;
        private bool _isBuiltIn = false;

        public RunCommand(IPolicySuiteRunner policySuiteRunner)
        {
            _policySuiteRunner = policySuiteRunner;
        }

        public override ValidationResult Validate(CommandContext context, RunCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileOrDirectory))
                return ValidationResult.Error("Please specify a path of a policy file, policy suite or directory.");

            if (settings.FileOrDirectory.StartsWith("builtin\\", StringComparison.OrdinalIgnoreCase))
            {
                _isBuiltIn = true;
            }
            else
            {
                _isDirectory = File.GetAttributes(settings.FileOrDirectory).HasFlag(FileAttributes.Directory);

                if (_isDirectory && !Directory.Exists(settings.FileOrDirectory))
                    return ValidationResult.Error($"Specified directory '{settings.FileOrDirectory}' does not exist.");
                else if (settings.FileOrDirectory.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
                {
                    _isBuiltIn = true;
                }
                else if (!File.Exists(settings.FileOrDirectory))
                    return ValidationResult.Error($"Specified file '{settings.FileOrDirectory}' does not exist.");
                else
                {
                    var fi = new FileInfo(settings.FileOrDirectory);

                    if (fi.Extension.Equals(FileExtensions.PolicySuiteExtension, StringComparison.OrdinalIgnoreCase))
                        _isPolicySuite = true;
                    else if (!fi.Extension.Equals(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                        return ValidationResult.Error($"Filename must end with either '{FileExtensions.PolicyFileExtension}' or '{FileExtensions.PolicySuiteExtension}'.");
                }
            }

            return base.Validate(context, settings);
        }

        public override async Task<int> ExecuteAsync(CommandContext context, RunCommandSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.FileOrDirectory))
                return -1;

            await Task.Yield();

            var policy = LoadPolicySuite(settings.FileOrDirectory);

            AnsiConsole.WriteLine(new SerializerBuilder().WithIndentedSequences().Build().Serialize(policy));

            var results = await _policySuiteRunner.RunPoliciesAsync(policy, default);
            return 0;
        }

        private PolicySuite LoadPolicySuite(string name)
        {
            PolicySuite result;

            if (_isBuiltIn)
                result = LoadPolicySuiteFromName(name);
            else if (_isPolicySuite)
                result = LoadPolicySuiteFromName(name);
            else
            {
                result = new PolicySuite
                {
                    Scopes = new[]
                    {
                        new PolicySuiteScope {Type = "AWS", Name = "AWS", Regions = PolicyRegions.AWS.All, Policies = new[] {name}},
                        new PolicySuiteScope {Type = "Azure", Name = "Azure", Regions = PolicyRegions.Azure.All, Policies = new[] {name}},
                    }
                };
            }

            return result;
        }

        private static PolicySuite LoadPolicySuiteFromName(string name)
        {
            const string builtInAws = "builtin\\aws\\";
            const string builtInAzure = "builtin\\azure\\";

            if (!name.EndsWith(".suite", StringComparison.OrdinalIgnoreCase))
                throw new Exception("Invalid policy suite");

            if (name.StartsWith("builtin", StringComparison.OrdinalIgnoreCase))
            {
                if (name.StartsWith(builtInAws, StringComparison.OrdinalIgnoreCase))
                {
                    var prefix = "Aspect.BuiltIn.AWS.";
                    if (LoadResourcesFromAssemblyByKeyPrefix(prefix).TryGetValue(name.Substring(builtInAws.Length), out var suite))
                        return LoadFromString(suite);
                }
                else if (name.StartsWith(builtInAzure, StringComparison.OrdinalIgnoreCase))
                {
                    var prefix = "Aspect.BuiltIn.Azure.";
                    if (LoadResourcesFromAssemblyByKeyPrefix(prefix).TryGetValue(name.Substring(builtInAzure.Length), out var suite))
                        return LoadFromString(suite);
                }
            }
            else
            {
                return LoadFromString(File.ReadAllText(name));
            }

            throw new Exception("Policy not found");

            Dictionary<string, string> LoadResourcesFromAssemblyByKeyPrefix(string prefix)
            {
                var assembly = typeof(RunCommand).Assembly;
                return assembly.GetManifestResourceNames()
                    .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(x => x.Substring(prefix.Length), x =>
                    {
                        // manifest stream cannot be null here, but the compiler doesn't know that
                        using var sr = new StreamReader(assembly.GetManifestResourceStream(x)!);
                        return sr.ReadToEnd();
                    }, StringComparer.OrdinalIgnoreCase);
            }

            PolicySuite LoadFromString(string policy)
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(UnderscoredNamingConvention.Instance)
                    .Build();

                return deserializer.Deserialize<PolicySuite>(policy);
            }
        }
    }
}
