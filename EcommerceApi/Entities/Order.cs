namespace EcommerceApi.Entities
{
    public enum OrderStatus
    {
        Pending,
        PaymentRecived,
        PaymentFailed
    }

    public class Order
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public Address ShipToAddress { get; set; } = new Address();
        public List<OrderItem> orderItems { get; set; } = new List<OrderItem>();
        public decimal Subtotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
