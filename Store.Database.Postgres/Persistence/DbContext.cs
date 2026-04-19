using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Database.Postgres.Persistence;

public class StoreContext(DbContextOptions<StoreContext> options, IConfiguration configuration) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<Cart> Carts { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
          optionsBuilder
      .UseNpgsql(configuration.GetConnectionString("Postgres"))
      .UseLoggerFactory(CreateLoggerFactory())
      .EnableSensitiveDataLogging()
      .UseAsyncSeeding(async (context, _, ct) =>
      {
        if (!await context.Set<Product>().AnyAsync(ct))
        {
          await context.Set<Product>().AddRangeAsync(
            new Product { Name = "Gaming Mouse", Description = "Ergonomic wireless gaming mouse with adjustable DPI.", Price = 59.99m, Quantity = 25 },
            new Product { Name = "Mechanical Keyboard", Description = "Compact mechanical keyboard with RGB backlight.", Price = 89.50m, Quantity = 18 },
            new Product { Name = "27 Inch Monitor", Description = "27-inch IPS monitor with 1440p resolution.", Price = 249.00m, Quantity = 12 },
            new Product { Name = "USB C Hub", Description = "Seven-port USB-C hub with HDMI and power delivery.", Price = 44.99m, Quantity = 40 },
            new Product { Name = "Laptop Stand", Description = "Aluminum adjustable stand for laptops up to 17 inches.", Price = 34.75m, Quantity = 30 },
            new Product { Name = "Noise Cancelling Headphones", Description = "Over-ear Bluetooth headphones with active noise cancellation.", Price = 129.99m, Quantity = 14 },
            new Product { Name = "Webcam Full HD", Description = "1080p webcam with stereo microphone and privacy shutter.", Price = 69.90m, Quantity = 22 },
            new Product { Name = "Portable SSD 1TB", Description = "High-speed external SSD with USB 3.2 support.", Price = 119.00m, Quantity = 16 },
            new Product { Name = "Smartphone Charger", Description = "Fast 45W wall charger with dual USB-C ports.", Price = 27.49m, Quantity = 50 },
            new Product { Name = "Office Chair", Description = "Mesh ergonomic office chair with lumbar support.", Price = 199.95m, Quantity = 9 }
          );

          await context.SaveChangesAsync(ct);
        }

        if (!await context.Set<User>().AnyAsync(ct))
        {
          await context.Set<User>().AddAsync(
            new User { FirstName = "Rustam", LastName = "Karadzhaiev", Email = "belonoir@gmail.com", Role = UserRole.Admin }
          , ct);

          await context.SaveChangesAsync(ct);
        }
      });
    }
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }

  public ILoggerFactory CreateLoggerFactory()
  {
    return LoggerFactory.Create(builder => { builder.AddConsole(); });
  }
}
