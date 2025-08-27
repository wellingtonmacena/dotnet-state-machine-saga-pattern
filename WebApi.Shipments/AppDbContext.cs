using Microsoft.EntityFrameworkCore;
using WebApi.Shipments.Models;

namespace WebApi.Shipments;

public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Shipment> Shipments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(o => o.OrderId)
              .HasColumnName("order_id")
              .HasMaxLength(100)
              .IsRequired();

            entity.Property(s => s.Carrier).HasMaxLength(100);
            entity.Property(s => s.TrackingNumber).HasMaxLength(100);
            entity.Property(s => s.DestinationAddress).HasMaxLength(250);

            entity.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(50);

            entity.Property(o => o.CreatedAt)
         .HasColumnName("created_at")
         .IsRequired();
            entity.Property(o => o.UpdatedAt)
              .HasColumnName("updated_at");

            entity.Property(s => s.ShippedAt)
            .HasColumnName("shipped_at")
            .IsRequired();

            entity.Property(s => s.DeliveredAt)
            .HasColumnName("delivered_at")
           .IsRequired();

            entity.Property(s => s.FailedAt)
            .HasColumnName("failed_at")
           .IsRequired();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: Entity });
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in modifiedEntries) ((Entity)entry.Entity).MarkAsUpdated();
        return base.SaveChangesAsync(cancellationToken);
    }
}
