using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

namespace Aspect.Commands
{
    internal static class ExecutionDataExtensions
    {
        internal static string AskQuestion(this IExecutionData _, string question, bool allowEmpty)
        {
            var prompt = new TextPrompt<string>(question) {AllowEmpty = allowEmpty, PromptStyle = ColourPallet.Prompt};
            string answer;
            do
            {
                answer = AnsiConsole.Prompt(prompt).Trim();
            } while (!allowEmpty && string.IsNullOrWhiteSpace(answer));

            return answer;
        }

        internal static string AskQuestion(this IExecutionData data, string key, string question, bool allowEmpty)
        {
            var answer = AskQuestion(data, question, allowEmpty);
            data.Set(key, answer);
            return answer;
        }

        internal static string AskToSelect(this IExecutionData _, string question, IEnumerable<string> choices)
        {
            var prompt = new SelectionPrompt<string> { Title = question, HighlightStyle = ColourPallet.Selection };
            prompt.AddChoices(choices);

            return AnsiConsole.Prompt(prompt);
        }

        internal static string AskToSelect(this IExecutionData data, string key, string question, IEnumerable<string> choices)
        {
            var answer = AskToSelect(data, question, choices);
            data.Set(key, answer);
            return answer;
        }

        internal static IEnumerable<string> AskToSelectMultiple(this IExecutionData data, string key, string message, IEnumerable<string> choices, bool required)
        {
            var prompt = new MultiSelectionPrompt<string> {Title = message, Required = required, HighlightStyle = ColourPallet.Selection};
            prompt.AddChoices(choices.OrderBy(x => x));

            var answer = AnsiConsole.Prompt(prompt);
            data.Set(key, answer);
            return answer;
        }
    }
}
