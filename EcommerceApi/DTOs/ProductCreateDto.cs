using System.ComponentModel.DataAnnotations;

namespace EcommerceApi.DTOs
{
    public class ProductCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Range(0.1, 100000)]
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;

        [Range(0, 1000)]
        public int StockQuantity { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
