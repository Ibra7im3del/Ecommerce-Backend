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

        public class OrderCreateDto
        {
            public Address? ShippingAddress { get; set; }
            public List<CartItemDto>? Items { get; set; }
        }

        public class CartItemDto
        {
            public int Id { get; set; }
            public string? ProductName { get; set; }
            public string? PictureUrl { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (orderDto.Items == null || !orderDto.Items.Any())
            {
                return BadRequest("The cart is empty; an order cannot be created");
            }

            var orderItems = orderDto.Items.Select(item => new OrderItem
            {
                ProductId = item.Id,
                ProductName = item.ProductName ?? string.Empty,
                PictureUrl = item.PictureUrl ?? string.Empty,
                UnitPrice = item.Price,
                Quantity = item.Quantity
            }).ToList();

            var subtotal = orderItems.Sum(item => item.UnitPrice * item.Quantity);

            var order = new Order
            {
                BuyerEmail = userEmail!,
                ShipToAddress = orderDto.ShippingAddress ?? new Address(),
                orderItems = orderItems,
                Subtotal = subtotal,
                OrderDate = DateTime.UtcNow
            };

            _context.orders.Add(order);
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
