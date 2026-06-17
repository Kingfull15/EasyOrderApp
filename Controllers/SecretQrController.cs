using Microsoft.AspNetCore.Mvc;
using QRCoder;
using RestaurantOrderingApp.Models;

namespace RestaurantOrderingApp.Controllers
{
    public class SecretQrController : Controller
    {
        [HttpGet("/SecretQr")]
        public IActionResult Index(int tableNumber = 1)
        {
            if (tableNumber < 1)
            {
                tableNumber = 1;
            }

            var menuUrl = Url.Action("Index", "Menu", new { table = tableNumber }, Request.Scheme)
                ?? $"/Menu?table={tableNumber}";

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(menuUrl, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            var qrBytes = qrCode.GetGraphic(20);

            var model = new SecretQrViewModel
            {
                TableNumber = tableNumber,
                MenuUrl = menuUrl,
                QrCodeBase64 = Convert.ToBase64String(qrBytes)
            };

            return View(model);
        }
    }
}

