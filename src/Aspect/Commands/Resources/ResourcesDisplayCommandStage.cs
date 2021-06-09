using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Spectre.Console;

namespace Aspect.Commands.Resources
{
    internal sealed class ResourcesDisplayCommandStage : CommandStage<ResourcesCommand.Settings>
    {
        internal override Task<int?> InvokeAsync(IExecutionData data, ResourcesCommand.Settings settings)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            RenderProvider(provider);
            return Task.FromResult<int?>(default);
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
                table.AddRow(row.Name!, row.Cloud!, row.Description!, row.Docs!);

            AnsiConsole.Render(table);
        }

        private class AvailableType
        {
            public string? Name { get; init; }
            public string? Cloud { get; init; }
            public string? Description { get; init; }
            public string? Docs => $"https://cloudaspect.app/docs/{Cloud?.ToLowerInvariant()}/resources/{Name?.ToLowerInvariant()}/";
            public Type? Type { get; init; }
        }
    }
}
