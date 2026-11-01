using Microsoft.EntityFrameworkCore;
using Store.Notifications.Domain.Models;
using Store.Notifications.Infrastructure.Persistance.Configurations;

namespace Store.Notifications.Infrastructure.Persistance;

public class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        base.OnModelCreating(modelBuilder);
    }
} 