namespace RestaurantOrderingApp.Models
{
    public class SecretQrViewModel
    {
        public int TableNumber { get; set; }
        public string MenuUrl { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
    }
}

