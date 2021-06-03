using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Aspect.Formatters
{
    internal sealed class ConsoleFormatter : IFormatter
    {
        public ValueTask FormatAsync<T>(T entity) where T : class => FormatAsync(new[] {entity});

        public ValueTask FormatAsync<T>(IEnumerable<T> entities) where T : class
        {
            var type = entities.FirstOrDefault()?.GetType();
            if (type is null)
                return ValueTask.CompletedTask;

            var properties = type.GetProperties().OrderBy(x => x.Name).ToList();
            var table = new Table();
            foreach (var property in properties)
            {
                table.AddColumn(property.Name);
            }

            foreach (var entity in entities)
            {
                var cols = new List<IRenderable>();

                if (entity is IFormatProperties ifp)
                {
                    foreach (var property in properties)
                        cols.Add(new Text(ifp.Format(property.Name)));
                }
                else
                {
                    foreach (var property in properties)
                        cols.Add(new Text(property.GetMethod!.Invoke(entity, Array.Empty<object>())?.ToString() ?? ""));
                }

                table.AddRow(cols);
            }

            AnsiConsole.Render(table);

            return ValueTask.CompletedTask;
        }
    }
}
