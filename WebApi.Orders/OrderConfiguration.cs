using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.Orders
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Tabela
            builder.ToTable("orders");

            // Chave primária GUID
            builder.HasKey(o => o.Id);

            // Se quiser gerar o GUID no banco automaticamente
            builder.Property(o => o.Id)
                   .HasColumnName("id");
           
            // Outras colunas
            builder.Property(o => o.ProductId)
                   .HasColumnName("product_id")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(o => o.Quantity)
                   .HasColumnName("quantity")
                   .HasColumnType("integer")
                   .IsRequired();

            builder.Property(s => s.Status)
       .HasColumnName("status")
 
       .HasMaxLength(100)
       .IsRequired();

            builder.Property(o => o.TotalPrice)
                   .HasColumnName("total_price")
                   .HasColumnType("decimal")
                   .IsRequired();

            builder.Property(o => o.CreatedAt)
                   .HasColumnName("created_at")
                   .IsRequired();
            builder.Property(o => o.UpdatedAt)
              .HasColumnName("updated_at");
              
        }
    }
}
