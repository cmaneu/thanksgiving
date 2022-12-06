using Spectre.Console;
using Spectre.Console.Cli;
using System.Collections;
using System.Reflection;
using System.Security.Cryptography;

namespace thanks_dev.Commands;

internal class AboutCommand : Command<DefaultCommandSettings>
{
    public override int Execute(CommandContext context, DefaultCommandSettings settings)
    {
        AnsiConsole.Write(new FigletText("Thanks!").Color(Color.Cyan1));
        AnsiConsole.MarkupLine("Thanks[cyan]giving[/][red]![/] developer CLI :nut_and_bolt:");

        AnsiConsole.MarkupLineInterpolated($"Version: {Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

        if (!settings.Verbose.HasValue || !settings.Verbose.Value)
            return 0;

        var table = new Table();
        table.Title = new TableTitle("Environment variables", new Style(decoration: Decoration.Bold));
        table.AddColumn("Name");
        table.AddColumn(new TableColumn("Value"));

        System.Collections.IDictionary env = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry variable in env)
        {
            if (variable.Key.ToString().StartsWith("THANKS"))
            {
                table.AddRow(new Markup($"[green]{variable.Key}[/]"), new Markup(variable.Value.ToString()));
            }
            else if (variable.Key.ToString().StartsWith("ASPNET") || variable.Key.ToString().StartsWith("DOTNET"))
            {
                table.AddRow(new Markup($"[blue]{variable.Key}[/]"), new Markup(variable.Value.ToString()));
            }
            else
            {
                table.AddRow(variable.Key.ToString(), variable.Value.ToString());
            }
        }

        AnsiConsole.Write(table);

        return 0;
    }
}
