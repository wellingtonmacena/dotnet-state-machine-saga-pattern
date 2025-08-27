using Microsoft.EntityFrameworkCore;
using WebApi.Inventory.Models;

namespace WebApi.Inventory;

public class AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<StockItem> StockItems { get; set; }
    public DbSet<Storage> Storages { get; set; }
    public DbSet<Product> Products { get; internal set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.Manufacturer).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(200);
            entity.Property(p => p.UpdatedAt).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();
        });

        // StockItem
        modelBuilder.Entity<StockItem>(entity =>
        {
            // Relação com Product
            entity.HasOne(si => si.Product)
                  .WithMany() // Product não precisa de coleção de StockItems
                  .HasForeignKey(si => si.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relação com Storage
            entity.HasOne(si => si.Storage)
                  .WithMany(s => s.Items)  // Storage sabe que tem muitos StockItems
                  .HasForeignKey(si => si.StorageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Storage -> StockItems
        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasMany(s => s.Items)
                  .WithOne(si => si.Storage)
                  .HasForeignKey(si => si.StorageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        SeedingDatabase(modelBuilder);
    }

    private void SeedingDatabase(ModelBuilder modelBuilder)
    {
        Product p1 = new()
        {
            Id = Guid.Parse("d4f1c872-5b6a-4d3f-9f73-7e8c9fda9009"),
            Name = "Notebook Dell XPS 13",
            Description = "Ultrabook com tela 13.3”",
            Price = 8500.00m,
            Manufacturer = "Dell",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };
        Product p2 = new()
        {
            Id = Guid.Parse("3a1f2c44-5f6d-4e5e-9b3f-21a7e8d1c001"),
            Name = "iPhone 15 Pro",
            Description = "128GB, Titânio",
            Price = 9999.99m,
            Manufacturer = "Apple",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };
        Product p3 = new()
        {
            Id = Guid.Parse("7f4b8c36-9f1a-4b0a-9f92-1f0a3e6c2d55"),
            Name = "Monitor LG UltraWide",
            Description = "34 polegadas, 144Hz",
            Price = 2499.50m,
            Manufacturer = "LG",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };

        modelBuilder.Entity<Product>().HasData(p1, p2, p3);

        // Storage (estoque principal)
        Storage storage = new()
        {
            Id = Guid.Parse("c8e2f91a-6f2b-4c6e-81a1-6c3d8f7a9002"),
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };
        modelBuilder.Entity<Storage>().HasData(storage);

        // StockItems (ligados ao Storage)
        StockItem si1 = new()
        {
            Id = Guid.Parse("b27a1e56-1c47-4e89-ae63-2b49cfa49003"),
            ProductId = p1.Id,
            StorageId = storage.Id,   // 👈 agora sim
            Quantity = 10,
            Location = "Depósito A",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };
        StockItem si2 = new()
        {
            Id = Guid.Parse("9e8f4c72-27a9-4e5d-9e33-5a4b6d8a4004"),
            StorageId = storage.Id,
            ProductId = p2.Id,
            Quantity = 5,
            Location = "Depósito A",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };
        StockItem si3 = new()
        {
            Id = Guid.Parse("5c3b7a18-7e61-41d2-8322-0f9d4cfa5005"),
            StorageId = storage.Id,
            ProductId = p3.Id,
            Quantity = 20,
            Location = "Depósito B",
            CreatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 8, 26, 14, 0, 0, DateTimeKind.Utc)
        };

        modelBuilder.Entity<StockItem>().HasData(si1, si2, si3);

    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry> modifiedEntries = ChangeTracker.Entries().Where(e => e is { State: EntityState.Modified, Entity: Entity });
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry? entry in modifiedEntries) ((Entity)entry.Entity).MarkAsUpdated();
        return base.SaveChangesAsync(cancellationToken);
    }
}
