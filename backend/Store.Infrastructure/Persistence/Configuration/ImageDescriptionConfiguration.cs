using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Domain.Models;

namespace Store.Infrastructure.Persistence.Configuration;

public class ImageDescriptionConfiguration : IEntityTypeConfiguration<ImageDescription>
{
    public void Configure(EntityTypeBuilder<ImageDescription> builder)
    {
        builder.ToTable("images_description");
    }
}