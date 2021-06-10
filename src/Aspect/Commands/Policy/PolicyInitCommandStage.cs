using System.IO;
using Spectre.Console;

namespace Aspect.Commands.Policy
{
    internal abstract class PolicyInitCommandStage : CommandStage<PolicyInitCommand.Settings>
    {
        protected static void WriteFile(PolicyInitCommand.Settings settings, string content)
        {
            if (settings.Display.GetValueOrDefault(true))
                AnsiConsole.WriteLine(content);

            if (!string.IsNullOrWhiteSpace(settings.FileName))
            {
                using var file = File.CreateText(settings.FileName);
                file.Write(content);
                file.Flush();
                file.Close();
            }
        }
    }
}
