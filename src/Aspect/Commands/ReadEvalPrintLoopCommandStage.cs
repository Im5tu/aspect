using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal abstract class ReadEvalPrintLoopCommandStage<T> : CommandStage<T>
        where T : CommandSettings
    {
        private readonly bool _emptyMeansExit;
        private readonly string _statementPrompt;

        protected ReadEvalPrintLoopCommandStage(bool emptyMeansExit, string statementPrompt = "Enter statement:")
        {
            _emptyMeansExit = emptyMeansExit;
            _statementPrompt = statementPrompt;
        }

        internal override async Task<int?> InvokeAsync(IExecutionData data, T settings)
        {
            await InitializeAsync(data);

            var commands = GetCommands().ToDictionary(x => x.Prefix, x => x, StringComparer.OrdinalIgnoreCase);
            var userCommands = commands.Values.Where(x => !x.IsHidden)
                .OrderBy(x => x.Prefix)
                .Concat(new [] { new REPLCommand("exit", (_, _) => Task.CompletedTask, "Exit the REPL interface")})
                .ToList();
            var exit = false;
            do
            {
                await OnNextIterationAsync(data);

                if (userCommands.Count > 1)
                {
                    Write($"Available commands: {string.Join(", ", userCommands.Select(x => x.Prefix))}", ColourPallet.Primary);
                    foreach (var cmd in userCommands)
                        Write($"  - {cmd.Prefix}: {cmd.Description}", ColourPallet.Primary);
                }

                var input = data.AskQuestion(_statementPrompt, _emptyMeansExit);
                exit = "exit".Equals(input.Trim(), StringComparison.OrdinalIgnoreCase) || _emptyMeansExit && string.IsNullOrWhiteSpace(input);

                if (exit)
                    break;

                AnsiConsole.Clear();

                var replCmd = commands.FirstOrDefault(x => input.StartsWith(x.Key, StringComparison.OrdinalIgnoreCase)).Value;
                if (replCmd is { })
                    await replCmd.Executor(input.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries), data);
                else
                    await ExecuteDefaultCommandAsync(input, data.CreateScope());
            } while (!exit);

            return 0;
        }

        protected abstract IEnumerable<REPLCommand> GetCommands();

        protected abstract Task ExecuteDefaultCommandAsync(string input, IExecutionData data);

        protected virtual Task InitializeAsync(IExecutionData data) => Task.CompletedTask;
        protected virtual Task OnNextIterationAsync(IExecutionData data) => Task.CompletedTask;

        internal record REPLCommand(string Prefix, Func<string[], IExecutionData, Task> Executor, string Description, bool IsHidden = false);
    }
}
