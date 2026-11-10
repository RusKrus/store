using Microsoft.EntityFrameworkCore;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Infrastructure.Extensions;

public static class OptionsBuilderSeedExtensions
{
  public static DbContextOptionsBuilder SeedDb(this DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
    {
      if (!await context.Set<Product>().AnyAsync(ct))
      {
          await context.Set<Product>().AddRangeAsync(
              new Product(
                  "Gaming Mouse",
                  "Ergonomic wireless gaming mouse with adjustable DPI.",
                  59.99m,
                  25,
                  ProductCategory.Peripherals
              ),

              new Product(
                  "Mechanical Keyboard",
                  "Compact mechanical keyboard with RGB backlight.",
                  89.50m,
                  18,
                  ProductCategory.Peripherals
              ),

              new Product(
                  "27 Inch Monitor",
                  "27-inch IPS monitor with 1440p resolution.",
                  249.00m,
                  12,
                  ProductCategory.Monitors
              ),

              new Product(
                  "USB C Hub",
                  "Seven-port USB-C hub with HDMI and power delivery.",
                  44.99m,
                  40,
                  ProductCategory.ComputerComponents
              ),

              new Product(
                  "Laptop Stand",
                  "Aluminum adjustable stand for laptops up to 17 inches.",
                  34.75m,
                  30,
                  ProductCategory.Accessories
              ),

              new Product(
                  "Noise Cancelling Headphones",
                  "Over-ear Bluetooth headphones with active noise cancellation.",
                  129.99m,
                  14,
                  ProductCategory.AudioEquipment
              ),

              new Product(
                  "Webcam Full HD",
                  "1080p webcam with stereo microphone and privacy shutter.",
                  69.90m,
                  22,
                  ProductCategory.Peripherals
              ),

              new Product(
                  "Portable SSD 1TB",
                  "High-speed external SSD with USB 3.2 support.",
                  119.00m,
                  16,
                  ProductCategory.StorageDevices
              ),

              new Product(
                  "Smartphone Charger",
                  "Fast 45W wall charger with dual USB-C ports.",
                  27.49m,
                  50,
                  ProductCategory.Accessories
              ),

              new Product(
                  "Office Chair",
                  "Mesh ergonomic office chair with lumbar support.",
                  199.95m,
                  9,
                  ProductCategory.OfficeEquipment
              )
          );
      }

      if (!await context.Set<User>().AnyAsync(u => u.Role == UserRole.SuperAdmin, ct))
      {
        await context.Set<User>().AddAsync(
          new User(
            "Root",
            "Admin",
            "belonoir@gmail.com",
            "password",
            UserRole.SuperAdmin
          ) ,ct);
      }

      await context.SaveChangesAsync(ct);
    });
    return optionsBuilder;
  }
}