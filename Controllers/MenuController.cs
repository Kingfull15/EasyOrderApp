using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingApp.Extensions;
using RestaurantOrderingApp.Models;
using RestaurantOrderingApp.Services;

namespace RestaurantOrderingApp.Controllers
{
    public class MenuController : Controller
    {
        private readonly IDataService _dataService;

        public MenuController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("/Menu")]
        [HttpGet("/Menu/Index")]
        public async Task<IActionResult> Index(int? table)
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

            var menuItems = await _dataService.GetMenuItemsAsync();
            var groupedMenu = menuItems.GroupBy(item => item.Category).ToDictionary(g => g.Key, g => g.ToList());

            return View(groupedMenu);
        }

        [HttpPost]
        public async Task<IActionResult> AddToOrder(int menuItemId, int quantity)
        {
            var menuItem = await _dataService.GetMenuItemAsync(menuItemId);
            var tableNumber = HttpContext.Session.GetInt32("TableNumber") ?? 1;

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

            return Redirect($"/Menu/Index?table={tableNumber}");
        }
        [HttpPost("/Menu/DeleteOrderItem")]
        public IActionResult DeleteOrderItem(int menuItemId, int quantityToRemove = 1, int? table = null)
        {
            var tableNumber = table ?? HttpContext.Session.GetInt32("TableNumber") ?? 1;
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();
            if (quantityToRemove <= 0)
            {
                return RedirectToAction(nameof(ViewOrder), new { table = tableNumber });
            }

            var itemToRemove = orderItems.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (itemToRemove != null)
            {
                itemToRemove.Quantity -= quantityToRemove;

                if (itemToRemove.Quantity <= 0)
                {
                    orderItems.Remove(itemToRemove);
                }

                HttpContext.Session.SetObjectAsJson("CurrentOrder", orderItems);
            }

            return RedirectToAction(nameof(ViewOrder), new { table = tableNumber });
        }

        [HttpGet("/Menu/ViewOrder")]
        public IActionResult ViewOrder(int? table = null)
        {
            var tableNumber = table ?? HttpContext.Session.GetInt32("TableNumber") ?? 1;
            HttpContext.Session.SetInt32("TableNumber", tableNumber);
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            ViewBag.TableNumber = tableNumber;
            return View(orderItems);
        }

        [HttpPost("/Menu/SubmitOrder")]
        public async Task<IActionResult> SubmitOrder(int? table = null)
        {
            var tableNumber = table ?? HttpContext.Session.GetInt32("TableNumber") ?? 1;
            HttpContext.Session.SetInt32("TableNumber", tableNumber);
            var orderItems = HttpContext.Session.GetObjectFromJson<List<OrderItem>>("CurrentOrder") ?? new List<OrderItem>();

            if (orderItems.Any())
            {
                var order = new Order
                {
                    TableNumber = tableNumber,
                    Items = orderItems
                };

                await _dataService.AddOrderAsync(order);
                HttpContext.Session.Remove("CurrentOrder");
            }

            ViewBag.TableNumber = tableNumber;
            return View("OrderConfirmed");
        }
    }

}
