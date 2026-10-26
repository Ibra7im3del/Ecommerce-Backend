using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcommerceApi.Data;
using EcommerceApi.Models;

namespace EcommerceApi.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WishlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // جلب قائمة المفضلة
        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var items = await _context.WishlistItems
                .Include(w => w.Product)
                .ToListAsync();
            return Ok(items);
        }

        // إضافة منتج للمفضلة
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItem item)
        {
            var exists = await _context.WishlistItems
                .AnyAsync(w => w.ProductId == item.ProductId);

            if (exists) return BadRequest("Product already in wishlist");

            _context.WishlistItems.Add(item);
            await _context.SaveChangesAsync();

            // إرجاع العنصر مع بيانات المنتج
            var addedItem = await _context.WishlistItems
                .Include(w => w.Product)
                .FirstOrDefaultAsync(w => w.Id == item.Id);

            return Ok(addedItem);
        }

        // حذف منتج من المفضلة
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(w => w.ProductId == productId);

            if (item == null) return NotFound("Item not found");

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Removed successfully" });
        }
    }
}
