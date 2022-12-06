//#define ATTACH_DEBUGGER
using Spectre.Console;
using System.Diagnostics;
using thanks_dev.Commands;

#if ATTACH_DEBUGGER
AnsiConsole.MarkupLine(":warning: [red]Attach debugger now :bug:[/]");
Console.WriteLine("Attach debugger now");
Console.ReadKey();
#endif


var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<AboutCommand>("about")
        .WithDescription("Displays CLI version information.");


    config.AddBranch("api", api =>
    {
        api.AddCommand<HealthCheckCommand>("health")
            .WithDescription("Execute a health check on a deployed API")
            .WithExample(new[] { "api","health", "http://localhost:8080" });
    });
});

return app.Run(args);