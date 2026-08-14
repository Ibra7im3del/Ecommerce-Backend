namespace EcommerceApi.Entities
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;

        public List<CartItem> items { get; set; } = new List<CartItem>();
    }
}
