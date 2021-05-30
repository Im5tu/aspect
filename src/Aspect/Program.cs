using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.IdentityManagement;
using Amazon.SecurityToken;
using Aspect.Abstractions;
using Aspect.Commands;
using Aspect.Providers.AWS;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Resources.EC2;
using Newtonsoft.Json;
using Spectre.Console.Cli;

namespace Aspect
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.Settings.ApplicationName = "aspect";

                config.AddCommand<AutoCompleteCommand>("autocomplete")
                    .WithDescription("Generate an autocomplete script for either powershell or bash.");

                config.AddCommand<LanguageServerCommand>("langserver")
                    .WithDescription("Starts the language server.");

                config.AddBranch("policy", p =>
                {
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

                    p.SetDescription("Create, list and validate policy documents.");
                });

                config.AddCommand<RunCommand>("run")
                    .WithDescription("Run a one or more policies against your cloud infrastructure.")
                    .WithExample(new[] {"run", "D:\\policies"});

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

        private static async Task<AwsAccount> GetCurrentAwsAccountAsync()
        {
            var accountProvider = new AWSAccountIdentityProvider(new AmazonSecurityTokenServiceClient(), new AmazonIdentityManagementServiceClient());
            return (AwsAccount) await accountProvider.GetAccountAsync(default);
        }

        private static async Task<IEnumerable<IResource>> GetSecurityGroupsAsync()
        {
            var serializerOptions = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var account = await GetCurrentAwsAccountAsync();
            Console.WriteLine(account.ToString());

            var sgLoader = new SecurityGroupResourceExplorer(account, new AmazonEC2Client(RegionEndpoint.EUWest1));

            Console.WriteLine();
            Console.WriteLine("Security Groups:");
            return await sgLoader.DiscoverResourcesAsync(default);
            //Console.WriteLine(JsonConvert.SerializeObject(groups.First(x => x.Tags.Count > 0), serializerOptions));
        }
    }
}
