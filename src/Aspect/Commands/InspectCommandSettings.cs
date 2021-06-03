using Spectre.Console.Cli;

namespace Aspect.Commands
{
    internal class InspectCommandSettings : CommandSettings
    {
        [CommandOption("--format")]
        public FormattingOption Formatter { get; init; }

        internal enum FormattingOption
        {
            Console,
            Json
        }
    }
}
