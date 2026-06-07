using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Store.Domain.Models;
using Store.Infrastructure.Persistance.Configuration;

namespace Store.Infrastructure.Persistence;

public class StoreContext(DbContextOptions<StoreContext> options) : DbContext(options)
{
  public DbSet<User> Users { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<Cart> Carts { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new UserConfiguration());
    modelBuilder.ApplyConfiguration(new CartConfiguration());
    modelBuilder.ApplyConfiguration(new CartItemConfiguration());
    modelBuilder.ApplyConfiguration(new OrderConfiguration());
    base.OnModelCreating(modelBuilder);
  }
}
