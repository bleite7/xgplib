namespace XgpLib.SyncService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHostedService<GameSyncWorker>();
        builder.Services.AddHostedService<GenreSyncWorker>();

        var host = builder.Build();
        host.Run();
    }
}
