﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aspect.Abstractions;
using Aspect.Extensions;
using Aspect.Formatters;
using Aspect.Policies.CompilerServices;
using Aspect.Policies.CompilerServices.CompilationUnits;
using Aspect.Policies.CompilerServices.Generator;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class InspectCommand : AsyncCommand<InspectCommand.Settings>
    {
        internal class Settings : CommandSettings
        {
        }

        private readonly IPolicyCompiler _policyCompiler;
        private readonly IReadOnlyDictionary<string,ICloudProvider> _cloudProviders;
        private readonly List<IResource> _resources = new List<IResource>();

        public InspectCommand(IReadOnlyDictionary<string, ICloudProvider> cloudProviders, IPolicyCompiler policyCompiler)
        {
            _cloudProviders = cloudProviders;
            _policyCompiler = policyCompiler;
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var provider = _cloudProviders[this.PromptOrDefault("Select cloud provider:", _cloudProviders.Keys, "AWS")];
            var regions = this.MultiSelect("Select region:", provider.GetAllRegions()).ToList();

            var (resourceName, resourceType) = GetResources(provider);
            var result = await LoadResources(provider, resourceType, regions);

            if (!result)
                return 1;

            await HandleCommands(resourceName, resourceType, provider, regions);
            return 0;
        }

        private async Task<bool> LoadResources(ICloudProvider provider, Type resourceType, List<string> regions)
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

                        foreach (var region in regions)
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
            var resources = provider.GetResources();
            var answer = this.PromptOrDefault("Select resource:", resources.Keys);
            return (answer, resources[answer]);
        }

        private async Task HandleCommands(string resourceName, Type resourceType, ICloudProvider provider, List<string> regions)
        {
            do
            {
                AnsiConsole.MarkupLine("Available commands: help, refresh, exit");
                string answer;
                do
                {
                    answer = AnsiConsole.Prompt(new TextPrompt<string>("Enter statement:"));
                } while (string.IsNullOrWhiteSpace(answer));

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

                if (answer.StartsWith("list", StringComparison.OrdinalIgnoreCase))
                {
                    // This command is purposely undocumented as the rendering is slow AF
                    int? count = null;
                    var index = answer.IndexOf(' ', StringComparison.Ordinal);
                    if (index > 0)
                        count = int.Parse(answer.Substring(index + 1));

                    if (count.HasValue)
                        await FormatResourceTable(_resources.Take(count.Value));
                    else
                        await FormatResourceTable(_resources);

                    continue;
                }

                if ("refresh".Equals(answer, StringComparison.OrdinalIgnoreCase))
                {
                    await LoadResources(provider, resourceType, regions);
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