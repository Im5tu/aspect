using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Formatters;
using Aspect.Policies;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.Generator;
using Aspect.Providers.AWS;
using Aspect.Providers.AWS.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class InspectCommand : AsyncCommand<InspectCommandSettings>
    {
        private readonly IPolicyCompiler _policyCompiler;
        private readonly IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;
        private readonly List<IResource> _resources = new List<IResource>();

        public InspectCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IPolicyCompiler policyCompiler)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, InspectCommandSettings settings)
        {
            var providerSelection = new SelectionPrompt<string> { Title = "Select cloud provider:" };
            providerSelection.AddChoices(_cloudProviders.Keys);
            var provider = _cloudProviders[AnsiConsole.Prompt(providerSelection)];

            var regionPrompt = new SelectionPrompt<string> { Title = "Select region:" };
            regionPrompt.AddChoices(provider.GetAllRegions());
            var region = AnsiConsole.Prompt(regionPrompt);

            var (resourceName, resourceType) = GetResources(provider);
            var result = await LoadResources(provider, resourceType, region);

            if (!result)
                return 1;

            await HandleCommands(resourceName, resourceType, provider, region);
            return 0;
        }

        private async Task<bool> LoadResources(ICloudProvider provider, Type resourceType, string region)
        {
            try
            {
                await AnsiConsole.Status()
                    .AutoRefresh(true)
                    .StartAsync("Loading resources...", async statusContext =>
                    {
                        _resources.Clear();

                        Action<string> updater = str =>
                        {
                            if (!str.EndsWith("...", StringComparison.Ordinal))
                                str = $"{str}...";

                            statusContext.Status(str);
                        };

                        _resources.AddRange(await provider.DiscoverResourcesAsync(region, resourceType, updater, default));
                    });
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return false;
            }
        }

        private (string resourceName, Type resourceType) GetResources(ICloudProvider provider)
        {
            var resourcePrompt = new SelectionPrompt<string>
            {
                Title = "Select resource to inspect:",
            };
            resourcePrompt.AddChoices(provider.GetResources().Keys);
            var answer = AnsiConsole.Prompt(resourcePrompt);
            return (answer, provider.GetResources()[answer]);
        }

        private async Task HandleCommands(string resourceName, Type resourceType, ICloudProvider provider, string region)
        {
            do
            {
                AnsiConsole.MarkupLine("Available commands: help, refresh, exit");
                var answer = AnsiConsole.Prompt(new TextPrompt<string>("Enter statement:"));

                if ("exit".Equals(answer, StringComparison.OrdinalIgnoreCase))
                    break;

                AnsiConsole.Clear();
                if ("help".Equals(answer, StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.MarkupLine($"Available properties for input '{resourceName}':");
                    AnsiConsole.MarkupLine(string.Join(Environment.NewLine, resourceType.GetProperties().OrderBy(x => x.Name).Select(x => $"  - {x.Name}")));
                    AnsiConsole.MarkupLine(string.Empty);
                    continue;
                }

                if ("list".Equals(answer, StringComparison.OrdinalIgnoreCase))
                {
                    await FormatResourceTable(_resources);
                    continue;
                }

                if ("refresh".Equals(answer, StringComparison.OrdinalIgnoreCase))
                {
                    await LoadResources(provider, resourceType, region);
                    continue;
                }

                await ExecutePolicy(answer, resourceName);
            } while (true);
        }

        private async Task ExecutePolicy(string input, string resourceName)
        {
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
            foreach (var resource in _resources)
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
                AnsiConsole.MarkupLine("[italic]No resources matched your specified input. If you are expecting a resource, try the 'refresh' command.[/]");
            }
        }

        private async Task FormatResourceTable(IEnumerable<IResource> resources)
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
                        await new ConsoleFormatter().FormatAsync(resources);
                    });
                    await tsk;
                });
        }
    }
}
