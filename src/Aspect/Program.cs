using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.EC2;
using Amazon.IdentityManagement;
using Amazon.SecurityToken;
using Aspect.Abstractions;
using Aspect.Policies.CompilerServices;
using Aspect.Providers.AWS;
using Aspect.Providers.AWS.Models;
using Aspect.Providers.AWS.Models.EC2;
using Aspect.Providers.AWS.Resources.EC2;
using Newtonsoft.Json;

namespace Aspect
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
                https://blog.elmah.io/how-to-create-a-colored-cli-with-system-commandline-and-spectre/

                Filetypes:
                    - .aspect                                               # A single policy document

                aspect policy init <file>                                   # Create new policy document
                aspect policy list <directory>                              # Displays all of the policies in the specified directory (recursive)
                aspect policy watch <file>                                  # Watches a specified file and continually checks that it's valid when it changes
                aspect policy validate <file>                               # Validates a specified policy document

                aspect run <directory>                                      # Loads all the policies from the directory and evaluates them

             */

            var compiler = new PolicyCompiler();
            //var securityGroups = (await GetSecurityGroupsAsync()).ToList();
            var securityGroups = new AwsSecurityGroup[] {null!}.ToList();

            try
            {
                if (!compiler.IsPolicyFileValid(@"D:\dev\temp\Aspect\temp\test.policy"))
                {
                    return;
                }

                var evaluator = compiler.GetFunctionForPolicy(new FileCompilationUnit(@"D:\dev\temp\Aspect\temp\test.policy"));
                for (var i = 0; i < 1; i++)
                {
                    var sw = Stopwatch.StartNew();

                    foreach (var sg in securityGroups)
                    {
                        var awsSG = (AwsSecurityGroup) sg;
                        var result = evaluator(sg);
                        Console.WriteLine($"{awsSG.Arn} - {result}");
                    }

                    sw.Stop();
                    Console.WriteLine($"Completed in {sw.ElapsedMilliseconds:0.00}ms ({(sw.ElapsedMilliseconds / securityGroups.Count)}ms per invocation)");
                }

                await Task.Yield();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
