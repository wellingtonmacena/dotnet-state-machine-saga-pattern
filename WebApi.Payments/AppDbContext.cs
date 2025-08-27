using Microsoft.EntityFrameworkCore;
using WebApi.Payments.Models;

namespace WebApi.Payments;

public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(o => o.OrderId)
             .HasColumnName("order_id")
             .HasMaxLength(100)
             .IsRequired();

            // Definir precisão para valores decimais
            entity.Property(p => p.Amount).HasPrecision(18, 2);

            // Opcional: definir enum como string
            entity.Property(p => p.Status)
                  .HasConversion<string>()
                  .HasMaxLength(50);

            entity.Property(p => p.PaymentMethod)
             .HasColumnName("payment_method")
                  .HasConversion<string>()
                  .HasMaxLength(50);

            // PaymentDate deve ser obrigatório
            entity.Property(p => p.PaymentDate)
             .HasColumnName("payment_date")
             .IsRequired();

            // TransactionCode opcional
            entity.Property(p => p.TransactionCode)
             .HasColumnName("transaction_code")
             .HasMaxLength(100);

            entity.Property(o => o.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();

            entity.Property(o => o.UpdatedAt)
              .HasColumnName("updated_at");
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: Entity });
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in modifiedEntries) ((Entity)entry.Entity).MarkAsUpdated();
        return base.SaveChangesAsync(cancellationToken);
    }
}
