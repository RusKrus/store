using System;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Models;

namespace Store.Database.Postgres.Persistance;

public class StoreContext : DbContext
{
  public StoreContext(DbContextOptions<StoreContext> options) : base(options)
  {
    
  }
  public DbSet<User> Users { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<Cart> Carts { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
  }
}
