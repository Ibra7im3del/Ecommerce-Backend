namespace EcommerceApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string UserName { get; set; } = "Customer";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
