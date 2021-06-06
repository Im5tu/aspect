using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Aspect.Abstractions;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal sealed class ResourcesCommand : Command<ResourcesCommand.Settings>
    {
        internal sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "[provider]")]
            public string? provider { get; init; }
        }

        private readonly IReadOnlyDictionary<string, ICloudProvider> _cloudProviders;

        public ResourcesCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders)
        {
            _cloudProviders = cloudProviders;
        }

        public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
        {
            if (_cloudProviders.TryGetValue(settings.provider ?? string.Empty, out var provider))
            {
                RenderProvider(provider);
            }
            else
            {
                foreach (var cp in _cloudProviders.OrderBy(x => x.Key))
                    RenderProvider(cp.Value);
            }

            return 0;
        }

        private void RenderProvider(ICloudProvider provider)
        {
            var resources = new List<AvailableType>();
            foreach (var type in provider.GetResources().Values)
            {
                var description = type.GetCustomAttribute<DescriptionAttribute>()?.Description ?? string.Empty;
                resources.Add(new AvailableType { Cloud = provider.Name, Name = type.Name, Description = description, Type = type });
            }

            RenderTable(resources);
        }

        private void RenderTable(List<AvailableType> availableTypes)
        {
            if (availableTypes.Count == 0)
                return;

            var columns = new[] {"Name", "Provider", "Description", "Docs"};
            var table = new Table().Expand();

            table.AddColumns(columns);

            foreach (var row in availableTypes.OrderBy(x => x.Name))
            {
                var properties = row.Type!.GetProperties().OrderBy(x => x.Name).Select(x =>
                {
                    var description = x.PropertyType.GetCustomAttribute<DescriptionAttribute>()?.Description;
                    return string.IsNullOrWhiteSpace(description) ? $"- {x.Name}" : $"- {x.Name}: {description}";
                });

                table.AddRow(row.Name!, row.Cloud!, row.Description!, row.Docs!);
            }

            AnsiConsole.Render(table);
        }

        private class AvailableType
        {
            public string? Name { get; init; }
            public string? Cloud { get; init; }
            public string? Description { get; init; }
            public string? Docs => $"https://cloudaspect.app/docs/{Cloud}/resources/{Name}/";
            public Type? Type { get; init; }
        }
    }
}
