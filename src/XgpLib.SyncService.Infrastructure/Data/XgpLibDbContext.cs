using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        var utcNow = DateTimeOffset.UtcNow;
        var modifiedBy = MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.LastModifiedBy = modifiedBy;
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.ModifiedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(e => e.CreatedAt).IsModified = false;
                entry.Entity.LastModifiedBy = modifiedBy;
                entry.Entity.ModifiedAt = utcNow;
            }
        }
    }
}
