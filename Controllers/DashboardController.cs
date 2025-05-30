using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingApp.Services;

namespace RestaurantOrderingApp.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var orders = DataService.GetAllOrders();
            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int orderId, string status)
        {
            DataService.UpdateOrderStatus(orderId, status);
            return RedirectToAction("Index");
        }

        public IActionResult TableView(int tableNumber)
        {
            var orders = DataService.GetOrdersByTable(tableNumber);
            ViewBag.TableNumber = tableNumber;
            return View(orders);
            
        }
        // In DashboardController.cs
        [HttpPost]
        public IActionResult DeleteOrder(int orderId)
        {
            DataService.DeleteOrder(orderId);
            return RedirectToAction("Index");
        }


    }

}
