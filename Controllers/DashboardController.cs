using Microsoft.AspNetCore.Mvc;
using RestaurantOrderingApp.Services;

namespace RestaurantOrderingApp.Controllers
{
    public class DashboardController : Controller
    {
        private const string SelectedDashboardTableKey = "SelectedDashboardTable";
        private readonly IDataService _dataService;

        public DashboardController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("/")]
        [HttpGet("/Dashboard")]
        public IActionResult Index()
        {
            var selectedTable = HttpContext.Session.GetInt32(SelectedDashboardTableKey);
            if (!selectedTable.HasValue)
            {
                return RedirectToAction("SelectTable");
            }

            return RedirectToAction(nameof(TableView));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            await _dataService.UpdateOrderStatusAsync(orderId, status);
            return RedirectToAction("Index");
        }

        [HttpGet("/TableView")]
        public async Task<IActionResult> TableView(int? tableNumber)
        {
            var selectedTable = HttpContext.Session.GetInt32(SelectedDashboardTableKey);
            if (!selectedTable.HasValue)
            {
                TempData["TableSelectError"] = "Please select a table first.";
                return RedirectToAction("SelectTable");
            }

            var activeTable = tableNumber ?? selectedTable.Value;
            if (activeTable < 1)
            {
                TempData["TableSelectError"] = "Please select a valid table number.";
                return RedirectToAction("SelectTable");
            }

            HttpContext.Session.SetInt32(SelectedDashboardTableKey, activeTable);

            var orders = await _dataService.GetOrdersByTableAsync(activeTable);
            ViewBag.TableNumber = activeTable;
            return View(orders);
        }

        [HttpGet("/SelectTable")]
        public IActionResult SelectTable()
        {
            return View();
        }

        [HttpPost("/SelectTable")]
        public IActionResult SelectTable(int tableNumber)
        {
            if (tableNumber < 1)
            {
                TempData["TableSelectError"] = "Please enter a valid table number.";
                return View();
            }

            HttpContext.Session.SetInt32(SelectedDashboardTableKey, tableNumber);

            return RedirectToAction(nameof(TableView), new { tableNumber });
        }

        // In DashboardController.cs
        [HttpPost]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            await _dataService.DeleteOrderAsync(orderId);
            return RedirectToAction("Index");
        }


    }

}
