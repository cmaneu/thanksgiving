using Spectre.Console;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace thanks_dev.Commands;

internal class HealthCheckCommand : AsyncCommand<HealthCheckSettings>
{
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] HealthCheckSettings settings)
    {
        // https://docs.docker.com/engine/reference/builder/#healthcheck
        // Docker requires either a return code of 0 or 1
        switch (settings.Type)
        {
            case "default":
            default:
                return await ExecuteSimpleHealthcheck(settings.APIEndpoint);
        }
        
    }

    private async Task<int> ExecuteSimpleHealthcheck(string? apiEndpoint)
    {
        HttpClient http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        try
        {
            var response = await http.GetAsync($"{apiEndpoint}/.core/healthcheck");
            response.EnsureSuccessStatusCode();
            Console.WriteLine("ok");
            return 0;
        }
        catch (Exception e)
        {
            AnsiConsole.WriteException(e);
            return 1;
        }
    }
}


internal class HealthCheckSettings : CommandSettings
{
    [CommandArgument(0, "[endpoint]")]
    [DefaultValue("default")]
    public string? APIEndpoint { get; set; }


    [CommandOption("-t|--type <type>")]
    [DefaultValue("default")]
    public string? Type { get; set; }

}