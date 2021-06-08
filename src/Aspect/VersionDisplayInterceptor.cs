using System;
using Aspect.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Aspect
{
    internal class DisplayInterceptor : ICommandInterceptor
    {
        public void Intercept(CommandContext context, CommandSettings settings)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            AnsiConsole.Clear();

            if (!context.Name.Equals("run", StringComparison.OrdinalIgnoreCase))
                AnsiConsole.MarkupLine($"[{ColourPallet.Primary.Foreground.ToMarkup()}]Aspect v{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}. Code: http://github.com/im5tu/aspect[/]");
        }
    }
}
