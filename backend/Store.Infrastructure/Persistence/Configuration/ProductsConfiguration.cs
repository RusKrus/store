using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Domain.Models;

namespace Store.Infrastructure.Persistence.Configuration;

public class ProductsConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.Property(product => product.Specs)
            .HasColumnName("specs")
            .HasColumnType("jsonb")
            .IsRequired();
    }
}