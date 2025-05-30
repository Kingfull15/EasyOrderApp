namespace RestaurantOrderingApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableNumber { get; set; }
        public DateTime OrderTime { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalAmount => Items.Sum(item => item.SubTotal);
        public string Status { get; set; } = "Pending";
    }

}
