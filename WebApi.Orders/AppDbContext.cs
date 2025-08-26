using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using WebApi.Orders.Sagas;

namespace WebApi.Orders;

[ExcludeFromCodeCoverage]
public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<ProductOrderingSagaData> SagaData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ProductOrderingSagaData>().HasKey(s => s.CorrelationId);
        modelBuilder.Entity<Order>().Property(s => s.Status).HasConversion<string>();
        modelBuilder.ApplyConfiguration(new OrderConfiguration()); 
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: Entity });
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in modifiedEntries) ((Entity)entry.Entity).MarkAsUpdated();
        return base.SaveChangesAsync(cancellationToken);
    }
}
