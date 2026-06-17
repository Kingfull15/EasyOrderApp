using Microsoft.EntityFrameworkCore;
using RestaurantOrderingApp.Data;
using RestaurantOrderingApp.Models;

namespace RestaurantOrderingApp.Services;

public class DataService : IDataService
{
    private readonly AppDbContext _dbContext;

    public DataService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<MenuItem>> GetMenuItemsAsync()
    {
        return _dbContext.MenuItems
            .Where(item => item.IsAvailable)
            .OrderBy(item => item.Category)
            .ThenBy(item => item.Name)
            .ToListAsync();
    }

    public Task<MenuItem?> GetMenuItemAsync(int id)
    {
        return _dbContext.MenuItems.FirstOrDefaultAsync(item => item.Id == id && item.IsAvailable);
    }

    public async Task AddOrderAsync(Order order)
    {
        order.OrderTime = DateTime.UtcNow;
        order.Status = string.IsNullOrWhiteSpace(order.Status) ? "Pending" : order.Status;

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
    }

    public Task<List<Order>> GetAllOrdersAsync()
    {
        return _dbContext.Orders
            .Include(order => order.Items)
            .OrderByDescending(order => order.OrderTime)
            .ToListAsync();
    }

    public Task<List<Order>> GetOrdersByTableAsync(int tableNumber)
    {
        return _dbContext.Orders
            .Include(order => order.Items)
            .Where(order => order.TableNumber == tableNumber)
            .OrderByDescending(order => order.OrderTime)
            .ToListAsync();
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(item => item.OrderId == orderId);
        if (order is null)
        {
            return;
        }

        order.Status = status;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(int orderId)
    {
        var order = await _dbContext.Orders.FirstOrDefaultAsync(item => item.OrderId == orderId);
        if (order is null)
        {
            return;
        }

        _dbContext.Orders.Remove(order);
        await _dbContext.SaveChangesAsync();
    }

}
