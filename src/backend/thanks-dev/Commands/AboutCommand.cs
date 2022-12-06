using Spectre.Console;
using Spectre.Console.Cli;
using System.Reflection;

namespace thanks_dev.Commands;

internal class AboutCommand : Command
{
    public override int Execute(CommandContext context)
    {
        AnsiConsole.Write(new FigletText("Thanks!").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("Thanks[cyan]giving[/][red]![/] developer CLI");

        AnsiConsole.MarkupLineInterpolated($"Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

        return 0;
    }
}
