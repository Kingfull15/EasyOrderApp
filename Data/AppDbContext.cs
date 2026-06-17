using Microsoft.EntityFrameworkCore;
using RestaurantOrderingApp.Models;

namespace RestaurantOrderingApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasKey(order => order.OrderId);

        modelBuilder.Entity<Order>()
            .Property(order => order.Status)
            .HasMaxLength(50);

        modelBuilder.Entity<Order>()
            .HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(orderItem => orderItem.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MenuItem>()
            .Property(item => item.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<OrderItem>()
            .Property(item => item.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "Espresso", Description = "", Price = 80m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 2, Name = "Machiato", Description = "", Price = 90m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 3, Name = "Cappuchino", Description = "", Price = 100m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 4, Name = "Coca Cola", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 5, Name = "Fanta", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 6, Name = "Sprite", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 7, Name = "Rosa", Description = "", Price = 80m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 8, Name = "Rosa gazirana", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true }
        );
    }
}

