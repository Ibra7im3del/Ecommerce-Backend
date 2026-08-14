using EcommerceApi.Data;
using EcommerceApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.ShoppingCarts
                .Include(c => c.items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId! };
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return Ok(cart);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ShoppingCart>> AddItemToCart(int productId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.products.FindAsync(productId);
            if (product == null) return NotFound("The product does not exist");

            var cart = await _context.ShoppingCarts
                .Include(c => c.items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new ShoppingCart { UserId = userId! };
                _context.ShoppingCarts.Add(cart);
            }

            var existingItem = cart.items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    PictureUrl = product.PictureUrl,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
            return Ok(cart);
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<ShoppingCart>> RemoveItem(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.ShoppingCarts
                .Include(c => c.items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return NotFound("The basket is empty");

            var item = cart.items.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cart.items.Remove(item);
                await _context.SaveChangesAsync();
            }

            return Ok(cart);
        }
    }
}

