using System.Collections.Generic;
using Spectre.Console;

namespace Aspect.Extensions
{
    internal static class ConsoleExtensions
    {
        internal static string PromptOrDefault(string message, IEnumerable<string> choices, string defaultValue = "")
        {
            var prompt = new SelectionPrompt<string> { Title = message };
            prompt.AddChoices(choices);

            if (prompt.Choices.Count == 0)
                return defaultValue;

            if (prompt.Choices.Count == 1)
                return prompt.Choices[0];

            return AnsiConsole.Prompt(prompt);
        }
    }
}
