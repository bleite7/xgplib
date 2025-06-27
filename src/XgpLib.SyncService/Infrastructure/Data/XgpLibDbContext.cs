using Microsoft.EntityFrameworkCore;

namespace XgpLib.SyncService.Infrastructure.Data;

public class XgpLibDbContext(DbContextOptions<XgpLibDbContext> options) : DbContext(options)
{
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Game> Games { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .Property(g => g.Id)
            .ValueGeneratedNever();

        modelBuilder.Entity<Genre>()
            .Property(g => g.Id)
            .ValueGeneratedNever();
    }
}
