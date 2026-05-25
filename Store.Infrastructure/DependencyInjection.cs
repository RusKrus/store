using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Auth;
using Store.Infrastructure.Repositories;


namespace Store.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<StoreContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(configuration.GetConnectionString("Postgres"))
                .EnableSensitiveDataLogging()
                .LogTo(
                    Console.WriteLine,
                    LogLevel.Information)
                .UseAsyncSeeding(async (context, _, ct) =>
                {
                    if (!await context.Set<Product>().AnyAsync(ct))
                    {
                        await context.Set<Product>().AddRangeAsync(
                            new Product(
                                "Gaming Mouse",
                                "Ergonomic wireless gaming mouse with adjustable DPI.",
                                59.99m,
                                25
                            ),

                            new Product(
                                "Mechanical Keyboard",
                                "Compact mechanical keyboard with RGB backlight.",
                                89.50m,
                                18
                            ),

                            new Product(
                                "27 Inch Monitor",
                                "27-inch IPS monitor with 1440p resolution.",
                                249.00m,
                                12
                            ),

                            new Product(
                                "USB C Hub",
                                "Seven-port USB-C hub with HDMI and power delivery.",
                                44.99m,
                                40
                            ),

                            new Product(
                                "Laptop Stand",
                                "Aluminum adjustable stand for laptops up to 17 inches.",
                                34.75m,
                                30
                            ),

                            new Product(
                                "Noise Cancelling Headphones",
                                "Over-ear Bluetooth headphones with active noise cancellation.",
                                129.99m,
                                14
                            ),

                            new Product(
                                "Webcam Full HD",
                                "1080p webcam with stereo microphone and privacy shutter.",
                                69.90m,
                                22
                            ),

                            new Product(
                                "Portable SSD 1TB",
                                "High-speed external SSD with USB 3.2 support.",
                                119.00m,
                                16
                            ),

                            new Product(
                                "Smartphone Charger",
                                "Fast 45W wall charger with dual USB-C ports.",
                                27.49m,
                                50
                            ),

                            new Product(
                                "Office Chair",
                                "Mesh ergonomic office chair with lumbar support.",
                                199.95m,
                                9
                            )
                        );
                    }
                });
        });
        return services;
    }


    public static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }

    public static IServiceCollection AddHasherService(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }

    public static IServiceCollection AddJwtService(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();
        return services;
    }
}