﻿using Aspect.Commands;
using Aspect.Dependencies;
using Aspect.Policies;
using Aspect.Providers.AWS;
using Aspect.Providers.Azure;
using Aspect.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Aspect
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandApp(RegisterServices());
            app.Configure(config =>
            {
                config.Settings.ApplicationName = "aspect";

                config.AddCommand<AutoCompleteCommand>("autocomplete")
                    .WithDescription("Generate an autocomplete script for either powershell or bash.")
                    .IsHidden();

#if DEBUG
                config.AddCommand<DocGenCommand>("docgen")
                    .WithDescription("Generate documents for the models for each cloud provider")
                    .IsHidden();
#endif

                config.AddCommand<LanguageServerCommand>("langserver")
                    .WithDescription("Starts the language server.")
                    .IsHidden();

                config.AddCommand<InspectCommand>("inspect")
                    .WithDescription("REPL style interface for listing resources that match the specified conditions");

                config.AddBranch("policy", p =>
                {
                    p.AddCommand<PolicyBuildCommand>("build")
                        .WithDescription("GUI to build policies");

                    p.AddCommand<PolicyInitCommand>("init")
                        .WithDescription("Create a new policy or policy suite")
                        .WithExample(new[] {"policy", "init", "D:\\policies\\my_new_policy.policy"});

                    p.AddCommand<PolicyListCommand>("list")
                        .WithAlias("ls")
                        .WithDescription("List policies in a directory")
                        .WithExample(new[] {"policy", "list", "D:\\policies"});

                    p.AddCommand<PolicyValidateCommand>("validate")
                        .WithDescription("Validate one or more policy documents")
                        .WithExample(new[] {"policy", "validate", "D:\\policies"});

                    p.AddCommand<PolicyViewCommand>("view")
                        .WithDescription("View the details of a .policy file")
                        .WithExample(new[] {"policy", "view", "D:\\policies\\my_policy.policy"});

                    p.SetDescription("Create, list and validate policy documents.");
                });

                config.AddCommand<RunCommand>("run")
                    .WithDescription("Run a one or more policies against your cloud infrastructure.")
                    .WithExample(new[] {"run", "D:\\policies"})
                    .IsHidden();

                config.AddCommand<ResourcesCommand>("resources")
                    .WithDescription("View the available types for all cloud providers")
                    .WithExample(new[] {"resources"})
                    .WithExample(new[] {"resources", "aws"});

                config.AddCommand<WatchCommand>("watch")
                    .WithDescription("Watch a specific directory for changes and validate them.")
                    .WithExample(new[] {"watch", "D:\\policies"});

#if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
#endif
            });
            return app.Run(args);
        }

        private static ITypeRegistrar RegisterServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<IPolicySuiteRunner, PolicySuiteRunner>()
                .AddSingleton<IPolicyLoader, PolicyLoader>()
                .AddCompilerService()
                .AddAWSCloudProvider()
                .AddAzureCloudProvider();

            return new MicrosoftDiTypeRegistrar(services);
        }
    }
}
