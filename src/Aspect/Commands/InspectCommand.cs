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
        private readonly Dictionary<Type, IResourceExplorer<AwsAccount, AwsAccountIdentifier>> _awsProviders = new();
        private readonly Dictionary<Type, IResourceExplorer> _azureProviders = new();
        private readonly List<IResource> _resources = new List<IResource>();
        private string? _resource = null;
        private string? _provider;

        public InspectCommand(IEnumerable<IResourceExplorer> resourceExplorers)
        {

            foreach (var provider in resourceExplorers)
            {
                if (provider is IResourceExplorer<AwsAccount, AwsAccountIdentifier> awsProvider)
                    _awsProviders[awsProvider.ResourceType] = awsProvider;
                // TODO :: Add Azure
            }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, InspectCommandSettings settings)
        {
            _provider = AnsiConsole.Prompt(new SelectionPrompt<string>
            {
                Title = "Select cloud provider:",
                Choices = {"AWS", "Azure"}
            });

            var resource = GetResources();
            if (resource is null)
                return 1;

            var result = await LoadResources();

            if (!result)
                return 1;

            await HandleCommands();
            return 0;
        }

        private async Task<bool> LoadResources()
        {
            try
            {
                await AnsiConsole.Status()
                    .AutoRefresh(true)
                    .StartAsync("Loading resources...", async statusContext =>
                    {
                        _resources.Clear();

                        if (_resource is null)
                            return;

                        Action<string> updater = str =>
                        {
                            if (!str.EndsWith("...", StringComparison.Ordinal))
                                str = $"{str}...";

                            statusContext.Status(str);
                        };

                        if (_provider == "AWS")
                        {
                            _resources.AddRange(await new AWSResourceLoader().LoadResourcesForType(_resource, statusUpdater: updater));
                        }
                    });
                return true;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
                return false;
            }
        }

        private string? GetResources()
        {
            var resourcePrompt = new SelectionPrompt<string>
            {
                Title = "Select resource to inspect:",
            };

            if (_provider == "Azure")
            {
                AnsiConsole.MarkupLine("[red]Azure is not currently supported by this interface.[/]");
                return null;
            }

            if (_provider == "AWS")
                resourcePrompt.AddChoices(Types.GetTypes(_provider).Select(x => x.Name));
            else
                return null;

            _resource = AnsiConsole.Prompt(resourcePrompt);
            return _resource;
        }

        private async Task HandleCommands()
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
                    AnsiConsole.MarkupLine($"Available properties for input '{_resource}':");
                    AnsiConsole.MarkupLine(string.Join(Environment.NewLine, Types.GetType(_resource!, _provider!)!.GetProperties().OrderBy(x => x.Name).Select(x => $"  - {x.Name}")));
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
                    await LoadResources();
                    continue;
                }

                await ExecutePolicy(answer);
            } while (true);
        }

        private async Task ExecutePolicy(string input)
        {
            var policy = $@"resource ""{_resource}""
validate {{
{string.Join(Environment.NewLine, input.Split("&&", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(x => "    " + x))}
}}";
            var cntx = new CompilationContext(new SourceTextCompilationUnit(policy));
            var func = PolicyCompiler.GetFunctionForPolicy(cntx);

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
