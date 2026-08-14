using EcommerceApi.Data;
using EcommerceApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Address shippingAddress)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var cart = await _context.ShoppingCarts
                .Include(c => c.items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.items.Any())
                return BadRequest("The cart is empty; an order cannot be created");

            var orderItems = cart.items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                PictureUrl = item.PictureUrl,
                UnitPrice = item.Price,
                Quantity = item.Quantity,
            }).ToList();

            var subtotal = orderItems.Sum(item => item.UnitPrice * item.Quantity);

            var order = new Order
            {
                BuyerEmail = userEmail!,
                ShipToAddress = shippingAddress,
                orderItems = orderItems,
                Subtotal = subtotal
            };

            _context.orders.Add(order);

            _context.CartItems.RemoveRange(cart.items);

            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersForUser()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var orders = await _context.orders
                .Include(o => o.orderItems)
                .Where(o => o.BuyerEmail == userEmail)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }
    }
}
