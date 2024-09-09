namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public decimal OrderTotal { get; set; }
        public DateTime OrderPlaced { get; set; }
    }
}
