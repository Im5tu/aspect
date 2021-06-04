using System.Collections.Generic;
using System.Linq;
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

        internal static IEnumerable<string> MultiSelect(string message, IEnumerable<string> choices)
        {
            var prompt = new MultiSelectionPrompt<string> {Title = message};
            prompt.AddChoices(choices.OrderBy(x => x));

            if (prompt.Choices.Count <= 1)
                return prompt.Choices;

            return AnsiConsole.Prompt(prompt);
        }
    }
}
