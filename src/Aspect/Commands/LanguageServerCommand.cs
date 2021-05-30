﻿using System.Threading.Tasks;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class LanguageServerCommand : AsyncCommand<LanguageServerCommandSettings>
    {
        public override Task<int> ExecuteAsync(CommandContext context, LanguageServerCommandSettings settings)
        {
            // https://github.com/matarillo/LanguageServerProtocol
            // https://code.visualstudio.com/api/language-extensions/language-server-extension-guide
            AnsiConsole.MarkupLine("[red bold]This command is not implemented yet[/]");
            return Task.FromResult(1);
        }
    }
}