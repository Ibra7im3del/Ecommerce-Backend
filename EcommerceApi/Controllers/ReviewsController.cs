using EcommerceApi.Data;
using EcommerceApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [Route("api/products/{productId}/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // جلب تقييمات منتج معين
        [HttpGet]
        public async Task<IActionResult> GetReviews(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return Ok(reviews);
        }

        // إضافة تقييم جديد للمنتج
        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, [FromBody] Review review)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound("Product not found");

            review.ProductId = productId;
            review.CreatedAt = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }
    }
}
