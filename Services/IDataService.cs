using RestaurantOrderingApp.Models;

namespace RestaurantOrderingApp.Services;

public interface IDataService
{
    Task<List<MenuItem>> GetMenuItemsAsync();
    Task<MenuItem?> GetMenuItemAsync(int id);
    Task AddOrderAsync(Order order);
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> GetOrdersByTableAsync(int tableNumber);
    Task UpdateOrderStatusAsync(int orderId, string status);
    Task DeleteOrderAsync(int orderId);
}

