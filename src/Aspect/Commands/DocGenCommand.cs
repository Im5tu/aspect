using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies;
using Aspect.Policies.BuiltIn;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal sealed class DocGenCommand : Command<DocGenCommand.Settings>
    {
        internal sealed class Settings : DirectorySettings
        {}

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;
        private readonly IBuiltInPolicyProvider _builtInPolicyProvider;

        public DocGenCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IBuiltInPolicyProvider builtInPolicyProvider)
        {
            _cloudProviders = cloudProviders;
            _builtInPolicyProvider = builtInPolicyProvider;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] DocGenCommand.Settings settings)
        {
            var directory = @"D:\dev\personal\im5tu\Aspect";
            if (!string.IsNullOrWhiteSpace(settings.Directory))
                directory = settings.Directory;

            directory = Path.Combine(directory, "docs\\content\\docs\\");

            GenerateResourceDocumentation(directory);

            GenerateBuiltinPolicyDocumentation(directory);

            return 0;
        }

        private void GenerateResourceDocumentation(string baseDirectory)
        {
            foreach (var provider in _cloudProviders.Values.OrderBy(x => x.Name))
                DocumentProvider(provider, baseDirectory);
        }

        private void GenerateBuiltinPolicyDocumentation(string directory)
        {
            var baseDirectory = Path.Combine(directory, "builtin");
            if (Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, true);

            Directory.CreateDirectory(baseDirectory);
            Thread.Sleep(250);

            // Index document for the root page
            using var basefs = File.CreateText(Path.Combine(baseDirectory, "_index.md"));
            basefs.WriteLine("+++");
            basefs.WriteLine($"title = \"BuiltIn Policies\"");
            basefs.WriteLine("weight = 5");
            basefs.WriteLine("+++");
            basefs.WriteLine();
            basefs.WriteLine("{{< childpages >}}");
            basefs.WriteLine();
            basefs.Flush();
            basefs.Close();

            var index = 1;
            var directoryIndex = 1;
            foreach (var resource in _builtInPolicyProvider.GetAllResources().OrderBy(x => x.Name))
            {
                var lastIndexOfEscape = resource.Name.IndexOf('\\', 9);
                var dir = Path.Combine(directory, resource.Name.Substring(0, lastIndexOfEscape));

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);

                    using var providerFS = File.CreateText(Path.Combine(dir, "_index.md"));
                    providerFS.WriteLine("+++");
                    providerFS.WriteLine($"title = \"{resource.Name.Substring(8, lastIndexOfEscape - 8)}\"");
                    providerFS.WriteLine($"weight = {directoryIndex++}");
                    providerFS.WriteLine("+++");
                    providerFS.WriteLine();
                    providerFS.WriteLine("{{< childpages >}}");
                    providerFS.WriteLine();
                    providerFS.Flush();
                    providerFS.Close();
                }


                var filename = resource.Name.Replace(FileExtensions.PolicyFileExtension, ".md", StringComparison.OrdinalIgnoreCase).Replace(FileExtensions.PolicySuiteExtension, ".md", StringComparison.OrdinalIgnoreCase);
                using var fs = File.CreateText(Path.Combine(directory, filename));
                fs.WriteLine("+++");
                fs.WriteLine($"title = \"{resource.Name.Substring(lastIndexOfEscape + 1)}\"");
                fs.WriteLine($"weight = {index++}");
                fs.WriteLine("+++");
                fs.WriteLine();
                fs.WriteLine("## Commands");
                fs.WriteLine();
                fs.WriteLine("{{< table style=\"table-striped\" >}}");
                fs.WriteLine("|Command|Description|");
                fs.WriteLine("|------|------|");
                fs.WriteLine($"|`aspect policy view {resource.Name}`|View the contents of the policy|");
                fs.WriteLine($"|`aspect policy validate {resource.Name}`|Validate the policy|");
                fs.WriteLine($"|`aspect run {resource.Name}`|Validate the policy|");
                fs.WriteLine("{{< /table >}}");
                fs.WriteLine();
                fs.WriteLine("## Policy Definition");

                if (resource.Name.EndsWith(FileExtensions.PolicyFileExtension, StringComparison.OrdinalIgnoreCase))
                    fs.WriteLine("{{< code lang=\"tf\" >}}");
                else
                    fs.WriteLine("{{< code lang=\"yml\" >}}");

                fs.WriteLine(resource.GetAllText());
                fs.WriteLine("{{< /code >}}");
                fs.WriteLine();
                fs.Flush();
                fs.Close();
            }
        }

        private static void DocumentProvider(ICloudProvider provider, string directory)
        {
            var basePath = Path.Combine(directory, $"{provider.Name}\\Resources\\");
            if (Directory.Exists(basePath))
                Directory.Delete(basePath, true);

            Thread.Sleep(100);
            Directory.CreateDirectory(basePath);

            var indexRows = new List<string>();

            // Write each resource file
            var index = 1;
            var accountType = new KeyValuePair<string, Type>(provider.AccountType.Name, provider.AccountType);
            var addressableResources = provider.GetResources().OrderBy(x => x.Key);
            foreach (var resource in new [] {accountType}.Concat(addressableResources))
            {
                using var fs = File.CreateText(Path.Combine(basePath, $"{resource.Key}.md"));
                var type = resource.Value;
                var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "Coming soon!";
                var docsUrl = $"/docs/{provider.Name}/resources/{type.Name}/".ToLowerInvariant();

                indexRows.Add($"|[{type.Name}]({docsUrl})|{description}|[Goto Docs]({docsUrl})|");

                // Build the font matter for the resource
                fs.WriteLine("+++");
                fs.WriteLine($"title = \"{resource.Key}\"");
                fs.WriteLine($"description = \"{description}\"");
                fs.WriteLine($"weight = {index++}");
                fs.WriteLine("+++");
                fs.WriteLine();
                WriteType(type, fs, provider, 2);

                fs.WriteLine("## Policy Template");
                fs.WriteLine($"This template will give you a quick head start on generating a policy for an {type.Name}:");
                fs.WriteLine();
                fs.WriteLine("{{< code lang=\"tf\" >}}");
                fs.WriteLine($"resource \"{type.Name}\"");
                fs.WriteLine();
                fs.WriteLine("validate {");
                fs.WriteLine();
                fs.WriteLine("}");
                fs.WriteLine("{{< /code >}}");


                var nestedTypes = GetNestedTypes(type).Distinct().ToList();
                if (nestedTypes.Count > 0)
                {
                    fs.WriteLine("## Nested Types");
                    foreach (var nestedType in nestedTypes)
                    {
                        fs.WriteLine($"### {nestedType.GetFriendlyName()}");
                        WriteType(nestedType, fs, provider, 4);
                    }
                }

                fs.Flush();
                fs.Close();
            }

            // Write the index file
            using var ifs = File.CreateText(Path.Combine(basePath, "_index.md"));
            var desc = $"Available resources for {provider.Name}:";
            ifs.WriteLine("+++");
            ifs.WriteLine("title = \"Resources\"");
            ifs.WriteLine($"description = \"{desc}\"");
            ifs.WriteLine("weight = 10");
            ifs.WriteLine("+++");
            ifs.WriteLine();
            ifs.WriteLine();
            ifs.WriteLine();
            ifs.WriteLine("{{< table style=\"table-striped\" >}}");
            ifs.WriteLine("|Name|Description|Docs|");
            ifs.WriteLine("|----------|----------|----------|");
            foreach (var row in indexRows)
                ifs.WriteLine(row);
            ifs.WriteLine("{{< /table >}}");
            ifs.Flush();
            ifs.Close();
        }

        private static void WriteType(Type type, StreamWriter fs, ICloudProvider provider, int indentLevel = 3)
        {
            fs.WriteLine(type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty);
            fs.WriteLine();

            if (!type.IsAssignableTo(typeof(IResource)) && !type.IsNested)
            {
                fs.WriteLine("{{< alert style=\"warning\" >}} **Note:** _You will not be able to write a policy directly against this type._ {{< /alert >}}");
                fs.WriteLine();
            }

            fs.WriteLine($"{new string('#', indentLevel)} Properties");
            fs.WriteLine("{{< table style=\"table-striped\" >}}");
            fs.WriteLine("|Name|Description|Type|");
            fs.WriteLine("|----------|----------|----------|");
            foreach (var property in type.GetProperties().OrderBy(x => x.Name))
            {
                var propertyDescription = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
                if (property.PropertyType != typeof(string) && property.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                    propertyDescription += " There may be 0 or more entries in this collection.";

                if (!propertyDescription.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                    propertyDescription = propertyDescription.Trim() + ".";

                var pt = DescribeType(property.PropertyType);
                if (pt.StartsWith(provider.Name, StringComparison.OrdinalIgnoreCase) || (property.PropertyType.IsNested && property.DeclaringType == type))
                {
                    if (property.PropertyType.IsNested)
                        pt = $"[{pt}](#{pt.ToLowerInvariant()})";
                    else
                        pt = $"[{pt}](/docs/{provider.Name.ToLowerInvariant()}/resources/{pt.ToLowerInvariant()}/)";
                }


                fs.WriteLine($"|{property.Name}|{propertyDescription}|{pt}|");
            }
            fs.WriteLine("{{< /table >}}");
            fs.WriteLine();
        }

        private static string DescribeType(Type type)
        {
            if (type == typeof(string))
                return "String";
            if (type == typeof(bool))
                return "Boolean";
            if (type == typeof(Int16) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Decimal))
                return "Number";
            if (type.Name.StartsWith("KeyValuePair", StringComparison.OrdinalIgnoreCase))
                return "KeyValuePair";

            if (type.IsAssignableTo(typeof(IEnumerable)))
            {
                var types = type.GetGenericArguments().Select(x =>
                {
                    var name =x.GetFriendlyName();
                    if (x.IsNested)
                        return $"[{name}](#{name.ToLowerInvariant()})";

                    return name;
                });

                return $"Collection\\<{string.Join(",", types)}>";
            }

            if (type.Name.StartsWith("Nullable", StringComparison.OrdinalIgnoreCase))
                return DescribeType(type.GetGenericArguments()[0]);

            return type.GetFriendlyName();
        }

        private static IEnumerable<Type> GetNestedTypes(Type type)
        {
            var types = type.GetNestedTypes();
            foreach (var nt in types)
            {
                yield return nt;

                foreach (var nt2 in GetNestedTypes(nt))
                    yield return nt2;
            }
        }
    }
}
