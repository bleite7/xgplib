using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace XgpLib.SyncService.Infrastructure.Data;

/// <summary>
///
/// </summary>
public class XgpLibDbContextFactory : IDesignTimeDbContextFactory<XgpLibDbContext>
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public XgpLibDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<XgpLibDbContext>();
        optionsBuilder
            .EnableSensitiveDataLogging()
            .UseNpgsql(configuration.GetConnectionString("XgpLibPostgres"))
            .UseSnakeCaseNamingConvention();

        return new XgpLibDbContext(optionsBuilder.Options);
    }
}
