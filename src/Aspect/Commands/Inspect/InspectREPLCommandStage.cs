using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.CompilerServices.Generator;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Aspect.Commands.Inspect
{
    internal class InspectREPLCommandStage : ReadEvalPrintLoopCommandStage<InspectCommand.Settings>
    {
        private readonly IPolicyCompiler _policyCompiler;
        private static readonly string Resources = "Resources";

        public InspectREPLCommandStage(IPolicyCompiler policyCompiler)
            : base(false)
        {
            _policyCompiler = policyCompiler;
        }

        protected override IEnumerable<REPLCommand> GetCommands()
        {
            yield return new REPLCommand("help", (args, data) => ShowProperties(args, data), "Show the properties that are available for the input type");
            yield return new REPLCommand("refresh", (args, data) => LoadResources(args, data), "Query the cloud provider for the latest data");
            yield return new REPLCommand("regions", (args, data) => SwitchRegion(args, data), "Switch which regions you are looking at");
            yield return new REPLCommand("switch", (args, data) => SwitchResource(args, data), "Change the resource that you are looking at");
            yield return new REPLCommand("list", (args, data) => ListResources(args, data), string.Empty, true);
        }

        private async Task ListResources(string[] args, IExecutionData data)
        {
            AnsiConsole.Clear();

            var count = 20;
            if (args.Length == 2 && int.TryParse(args[1], out var temp) && temp != 0)
                count = temp;

            var resources = data.Get<List<IResource>>(Resources);

            if (count <= 0)
                count = resources.Count;


            await FormatResourceTable(resources.Take(count).ToList());
            Write($"Displaying {count}/{resources.Count} resources", ColourPallet.Selection);
        }

        protected override Task InitializeAsync(IExecutionData data) => LoadResources(Array.Empty<string>(), data);

        protected override Task OnNextIterationAsync(IExecutionData data)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            var regions = data.Get<IEnumerable<string>>(CommandStageConstants.Regions);
            var resource = data.Get<string>(CommandStageConstants.ResourceName);
            Write($"Cloud: {provider.Name} / Regions: {string.Join(", ", regions)} / Resource: {resource}", ColourPallet.Primary);
            return Task.CompletedTask;
        }

        protected override async Task ExecuteDefaultCommandAsync(string input, IExecutionData data)
        {
            var resources = data.Get<List<IResource>>(Resources);
            var resourceName = data.Get<string>(CommandStageConstants.ResourceName);
            var policy = $@"resource ""{resourceName}""
validate {{
{string.Join(Environment.NewLine, input.Split("&&", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(x => "    " + x))}
}}";
            var cntx = new CompilationContext(new SourceTextCompilationUnit(policy));
            var func = _policyCompiler.GetFunctionForPolicy(cntx);

            if (func is null)
            {
                AnsiConsole.MarkupLine("[yellow bold]Policy:[/]" + Environment.NewLine + Environment.NewLine + policy + Environment.NewLine);
                AnsiConsole.MarkupLine("[red bold]Result:[/]" + Environment.NewLine);
                cntx.WriteCompilationResultToConsole();
                return;
            }

            var passed = new List<IResource>();
            foreach (var resource in resources)
            {
                if (func(resource) == ResourcePolicyExecution.Passed)
                {
                    passed.Add(resource);
                }
            }

            if (passed.Count > 0)
            {
                await FormatResourceTable(passed);
            }
            else
            {
                AnsiConsole.MarkupLine("[orange1 italic]No resources matched your specified input. If you are expecting a resource, try the 'refresh' command.[/]");
            }
        }

        private static async Task FormatResourceTable(List<IResource> resources)
        {
            await AnsiConsole.Status()
                .AutoRefresh(true)
                .StartAsync("Formatting...", async sc =>
                {
                    sc.Status("Formatting...");
                    var tsk = Task.Run(async () =>
                    {
                        // force new thread
                        await Task.Delay(50);
                        await FormatAsync(resources);
                    });
                    await tsk;
                });
        }

        private static ValueTask FormatAsync<T>(List<T> entities) where T : class
        {
            var type = entities.FirstOrDefault()?.GetType();
            if (type is null)
                return ValueTask.CompletedTask;

            var properties = type.GetProperties().OrderBy(x => x.Name).Where(x => x.Name != "Type").ToList();
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

        private async Task LoadResources(string[] args, IExecutionData data)
        {
            await AnsiConsole.Status()
                .AutoRefresh(true)
                .StartAsync("Loading resources...", async statusContext =>
                {
                    var discoveredResources = new List<IResource>();

                    Action<string> updater = str =>
                    {
                        if (!str.EndsWith("...", StringComparison.Ordinal))
                            str = $"{str}...";

                        statusContext.Status(str);
                    };

                    var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
                    var resourceType = data.Get<Type>(CommandStageConstants.ResourceType);
                    foreach (var region in data.Get<IEnumerable<string>>(CommandStageConstants.Regions))
                        discoveredResources.AddRange(await provider.DiscoverResourcesAsync(region, resourceType, updater, default));

                    data.Set(Resources, discoveredResources);
                });
        }

        private Task ShowProperties(string[] args, IExecutionData data)
        {
            var resourceName = data.Get<Type>(CommandStageConstants.ResourceName);
            var resourceType = data.Get<Type>(CommandStageConstants.ResourceType);

            Write($"Available properties for input '{resourceName}':", ColourPallet.Prompt);
            Write(string.Join(Environment.NewLine, resourceType.GetProperties().OrderBy(x => x.Name).Select(x => $"  - {x.Name}")), ColourPallet.Prompt);
            Write(string.Empty, ColourPallet.Prompt);

            return Task.CompletedTask;
        }

        private async Task SwitchResource(string[] args, IExecutionData data)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            var resources = provider.GetResources();
            var resource = data.AskToSelect(CommandStageConstants.ResourceName, "Select resource:", resources.Keys);
            data.Set(CommandStageConstants.ResourceType, resources[resource]);
            await LoadResources(args, data);
        }

        private async Task SwitchRegion(string[] args, IExecutionData data)
        {
            var provider = data.Get<ICloudProvider>(CommandStageConstants.CloudProvider);
            _ = data.AskToSelectMultiple(CommandStageConstants.Regions, "Select the regions to work with:", provider.GetAllRegions(), true);
            await LoadResources(args, data);
        }
    }
}
