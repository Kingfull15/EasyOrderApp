using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingApp.Extensions;
using RestaurantOrderingApp.Models;
using RestaurantOrderingApp.Services;

namespace RestaurantOrderingApp.Controllers
{
    public class MenuController : Controller
    {
        public IActionResult Index(int? table)
        {
            if (table.HasValue)
            {
                HttpContext.Session.SetInt32("TableNumber", table.Value);
                ViewBag.TableNumber = table.Value;
            }
            else
            {
                var savedTable = HttpContext.Session.GetInt32("TableNumber");
                if (savedTable.HasValue)
                {
                    ViewBag.TableNumber = savedTable.Value;
                }
                else
                {
                    ViewBag.TableNumber = 1; // Default table for testing
                }
            }

            var menuItems = DataService.GetMenuItems();
            var groupedMenu = menuItems.GroupBy(item => item.Category).ToDictionary(g => g.Key, g => g.ToList());

            return View(groupedMenu);
        }

        [HttpPost]
        public IActionResult AddToOrder(int menuItemId, int quantity)
        {
            var tableNumber = HttpContext.Session.GetInt32("TableNumber") ?? 1;
            var menuItem = DataService.GetMenuItem(menuItemId);

            if (menuItem != null && quantity > 0)
            {
                var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

                var existingItem = orderItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    orderItems.Add(new OrderItem
                    {
                        MenuItemId = menuItem.Id,
                        MenuItemName = menuItem.Name,
                        Price = menuItem.Price,
                        Quantity = quantity
                    });
                }

                HttpContext.Session.SetObjectAsJson("CurrentOrder", orderItems);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult DeleteOrderItem(int menuItemId)
        {
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            var itemToRemove = orderItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (itemToRemove != null)
            {
                orderItems.Remove(itemToRemove);
                HttpContext.Session.SetObjectAsJson("CurrentOrder", orderItems);
            }

            return RedirectToAction("ViewOrder");
        }

        public IActionResult ViewOrder()
        {
            var tableNumber = HttpContext.Session.GetInt32("TableNumber") ?? 1;
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            ViewBag.TableNumber = tableNumber;
            return View(orderItems);
        }

        [HttpPost]
        public IActionResult SubmitOrder()
        {
            var tableNumber = HttpContext.Session.GetInt32("TableNumber") ?? 1;
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            if (orderItems.Any())
            {
                var order = new Order
                {
                    TableNumber = tableNumber,
                    Items = orderItems
                };

                DataService.AddOrder(order);
                HttpContext.Session.Remove("CurrentOrder");
            }

            return View("OrderConfirmed");
        }
    }

}
