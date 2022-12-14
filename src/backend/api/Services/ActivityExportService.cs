using api.Adapters;
using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using System.Formats.Tar;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace api.Services;

public class ActivityExportService
{
    private ThanksDBContext _dbContext;

    public ActivityExportService(ThanksDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Stream?> ExportUserActivitesAsTar(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user is not User)
            return null;

        List<Activity> userActivities = await _dbContext.Activities.Include(a => a.Organization).Where(a => a.VolunteerId == userId).ToListAsync();

        int fileIndex = 1;

        var rootFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(rootFolder);
        Directory.CreateDirectory(Path.Combine(rootFolder,"root"));
        foreach(var activitiesGroup in userActivities.GroupBy(a => a.OrganizationId))
        {
            var report = new OrganizationActivityReport(user, activitiesGroup.First().Organization, activitiesGroup);
            await File.WriteAllTextAsync(Path.Combine(rootFolder,"root",$"{fileIndex}.txt"),report.ExportReport());
        }

        var tarFilePath = Path.Combine(rootFolder, "export.tar");
        await TarFile.CreateFromDirectoryAsync(
            sourceDirectoryName: Path.Combine(rootFolder, "root"),
            destinationFileName: tarFilePath,
            includeBaseDirectory: false
          );
        
        using FileStream tarFileStream = File.OpenRead(tarFilePath);
        // TODO: Check how this stream is disposed by Results.Stream https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/responses?view=aspnetcore-7.0#stream7
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.results.stream?view=aspnetcore-7.0
        MemoryStream compressStream = new MemoryStream();

        //GZipStream compressor = new(compressStream, CompressionMode.Compress);

        await tarFileStream.CopyToAsync(compressStream);

        compressStream.Position = 0;
        return compressStream;
    }
    
}

file record OrganizationActivityReport
{
    public User User { get; init; }
    public string OrganizationName { get; init; }

    public IEnumerable<Activity> Activities { get; init; }
    public OrganizationActivityReport(User user, Organization organization, IEnumerable<Activity> activities)
    {
        User = user;
        OrganizationName = organization.Name;
        Activities = activities;
    }

    public string ExportReport()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($$"""
2022{{User.FirstName.PadRight(25, ' ')[..25]}}{{User.LastName.PadRight(25, ' ')[..25]}}{{OrganizationName}}
1{{{Activities.Count()}}}{{DateTime.Now:yyyy-MM-dd}}
""");
        foreach (var a in Activities)
        {
            sb.AppendLine($"{a.Date:yyy-MM-dd}{a.Duration}");
        }
        sb.AppendLine($$"""2022{{Activities.Count()+2}}"""); // Add two lines of header
        return sb.ToString();
    }
}
