using Spectre.Console;
using thanks_dev.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<AboutCommand>("about")
        .WithDescription("Displays CLI version information.");
});

return app.Run(args);