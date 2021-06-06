using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Extensions
{
    internal static class ConsoleExtensions
    {
        internal static string? NullIfEmpty(this string str) => string.IsNullOrWhiteSpace(str) ? null : str.Trim();

        internal static string Ask(this ICommand _, string message, bool allowEmpty = false)
        {
            string answer;
            do
            {
                answer = AnsiConsole.Prompt(new TextPrompt<string>(message) {AllowEmpty = allowEmpty});
            } while (!allowEmpty && string.IsNullOrWhiteSpace(answer));

            return answer;
        }

        internal static string PromptOrDefault(this ICommand _, string message, IEnumerable<string> choices, string defaultValue = "")
        {
            var prompt = new SelectionPrompt<string> { Title = message };
            prompt.AddChoices(choices);

            if (prompt.Choices.Count == 0)
                return defaultValue;

            if (prompt.Choices.Count == 1)
                return prompt.Choices[0];

            return AnsiConsole.Prompt(prompt);
        }

        internal static IEnumerable<string> MultiSelect(this ICommand _, string message, IEnumerable<string> choices, bool required = true)
        {
            var prompt = new MultiSelectionPrompt<string> {Title = message, Required = required};
            prompt.AddChoices(choices.OrderBy(x => x));

            if (prompt.Choices.Count <= 1)
                return prompt.Choices;

            return AnsiConsole.Prompt(prompt);
        }
    }
}
