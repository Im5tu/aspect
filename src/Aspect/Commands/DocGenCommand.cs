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
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal sealed class DocGenCommand : Command<DocGenCommandSettings>
    {
        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public DocGenCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] DocGenCommandSettings settings)
        {
            var directory = @"D:\dev\personal\im5tu\Aspect";
            if (!string.IsNullOrWhiteSpace(settings.Directory))
                directory = settings.Directory;

            foreach (var provider in _cloudProviders.Values.OrderBy(x => x.Name))
                DocumentProvider(provider, directory);

            return 0;
        }

        private void DocumentProvider(ICloudProvider provider, string directory)
        {
            var basePath = Path.Combine(directory, $"docs\\content\\docs\\{provider.Name}\\Resources\\");
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
                fs.WriteLine("```");
                fs.WriteLine($"resource \"{type.Name}\"");
                fs.WriteLine();
                fs.WriteLine("validate {");
                fs.WriteLine();
                fs.WriteLine("}");
                fs.WriteLine("```");


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
            ifs.WriteLine("|Name|Description|Docs|");
            ifs.WriteLine("|----------|----------|----------|");
            foreach (var row in indexRows)
                ifs.WriteLine(row);

            ifs.Flush();
            ifs.Close();
        }

        private static void WriteType(Type type, StreamWriter fs, ICloudProvider provider, int indentLevel = 3)
        {
            fs.WriteLine(type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty);
            fs.WriteLine();

            if (!type.IsAssignableTo(typeof(IResource)) && !type.IsNested)
            {
                fs.WriteLine("**Note:** _You will not be able to write a policy directly against this type._");
                fs.WriteLine();
            }

            fs.WriteLine($"{new string('#', indentLevel)} Properties");
            fs.WriteLine("|Name|Description|Type|");
            fs.WriteLine("|----------|----------|----------|");
            foreach (var property in type.GetProperties().OrderBy(x => x.Name))
            {
                var propertyDescription = property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
                if (property.PropertyType != typeof(string) && property.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                    propertyDescription += " There may be 0 or more entries in this collection.";

                var pt = DescribeType(property.PropertyType);

                if (pt.StartsWith(provider.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (property.PropertyType.IsNested)
                        pt = $"[{pt}](#{pt.ToLowerInvariant()})";
                    else
                        pt = $"[{pt}](/docs/{provider.Name.ToLowerInvariant()}/resources/{pt.ToLowerInvariant()}/)";
                }


                fs.WriteLine($"|{property.Name}|{propertyDescription}|{pt}|");
            }
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

        private IEnumerable<Type> GetNestedTypes(Type type)
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
