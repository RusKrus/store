using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Notifications.Domain.Models;

namespace Store.Notifications.Infrastructure.Persistance.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Type).HasConversion<string>().IsRequired();
        builder.Property(n => n.Payload).HasColumnType("jsonb").IsRequired();
        builder.Property(n => n.CreatedAt).IsRequired();
        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.IsRead);
        builder.Property(n => n.UserId).IsRequired();
    }
}