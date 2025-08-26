using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Inventory;

[ExcludeFromCodeCoverage]
public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: Entity });
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in modifiedEntries) ((Entity)entry.Entity).MarkAsUpdated();
        return base.SaveChangesAsync(cancellationToken);
    }
}
