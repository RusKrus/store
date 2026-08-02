using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Persistance.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(x => x.Exchange)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.RoutingKey)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Payload)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.LastErrorMessage)
            .HasMaxLength(4000);

        builder.Ignore(x => x.IsDead);

        builder.HasIndex(x => new
        {
            x.ProcessedAtUtc,
            x.CreatedAtUtc
        });

    }
}