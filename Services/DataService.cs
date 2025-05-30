using RestaurantOrderingApp.Models;

namespace RestaurantOrderingApp.Services
{
    public static class DataService
    {
        private static List<MenuItem> _menuItems = new List<MenuItem>();
        private static List<Order> _orders = new List<Order>();
        private static int _nextOrderId = 1;

        static DataService()
        {
            InitializeMenuItems();
        }

        private static void InitializeMenuItems()
        {
            _menuItems = new List<MenuItem>
        {
            new MenuItem { Id = 1, Name = "Espresso", Description = "", Price = 80m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 2, Name = "Machiato", Description = "", Price = 90m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 3, Name = "Cappuchino", Description = "", Price = 100m, Category = "Coffee", IsAvailable = true },
            new MenuItem { Id = 4, Name = "Coca Cola", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 5, Name = "Fanta", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 6, Name = "Sprite", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 7, Name = "Rosa", Description = "", Price = 80m, Category = "Beverages", IsAvailable = true },
            new MenuItem { Id = 8, Name = "Rosa gazirana", Description = "", Price = 90m, Category = "Beverages", IsAvailable = true },

        };
        }

        public static List<MenuItem> GetMenuItems()
        {
            return _menuItems.Where(item => item.IsAvailable).ToList();
        }

        public static MenuItem GetMenuItem(int id)
        {
            return _menuItems.FirstOrDefault(item => item.Id == id);
        }

        public static void AddOrder(Order order)
        {
            order.OrderId = _nextOrderId++;
            order.OrderTime = DateTime.Now;
            _orders.Add(order);
        }

        public static List<Order> GetAllOrders()
        {
            return _orders.OrderByDescending(order => order.OrderTime).ToList();
        }

        public static List<Order> GetOrdersByTable(int tableNumber)
        {
            return _orders.Where(order => order.TableNumber == tableNumber).ToList();
        }

        public static void UpdateOrderStatus(int orderId, string status)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }
        public static void DeleteOrder(int orderId)
        {
            var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order != null)
            {
                _orders.Remove(order);
            }
        }
    }

}
