using Spectre.Console;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace thanks_dev.Commands;

internal partial class HealthCheckCommand : AsyncCommand<HealthCheckSettings>
{
    public override async Task<int> ExecuteAsync([NotNull] CommandContext context, [NotNull] HealthCheckSettings settings)
    {
        // https://docs.docker.com/engine/reference/builder/#healthcheck
        // Docker requires either a return code of 0 or 1
        string port = "80";

        // Use a regex to extract the port from a value like http://+:80
        if (Environment.GetEnvironmentVariable("ASPNETCORE_URLS") is not null)
        {
            port = FindListeningPortRegEx().Match(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")).Value;
        }        

        switch (settings.Type)
        {
            case "default":
            default:
                return await ExecuteSimpleHealthcheck($"{settings.APIEndpoint}:{port}");
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

    [GeneratedRegex("(?<=:)\\d+")]
    private static partial Regex FindListeningPortRegEx();
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