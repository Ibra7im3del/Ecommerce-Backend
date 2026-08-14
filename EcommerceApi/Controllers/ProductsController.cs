using EcommerceApi.Constants;
using EcommerceApi.Data;
using EcommerceApi.DTOs;
using EcommerceApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get: api/products

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _context.products
                .Include(p => p.Category)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    PictureUrl = p.PictureUrl,
                    StockQuantity = p.StockQuantity,
                    CategoryName = p.Category != null ? p.Category.Name : "Uncategorized"
                })
                .ToListAsync();

            return Ok(products);
        }

        // Get: api/product/5

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { message = "product Not Found" });

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                PictureUrl = product.PictureUrl,
                StockQuantity = product.StockQuantity,
                CategoryName = product.Category != null ? product.Category.Name : "Uncategorized"
            };

            return Ok(productDto);
        }

        // Post: api/products

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateDto dto)
        {
            var categoryExists = await _context.categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExists)
                return BadRequest(new { message =  "Category Id Not Correct Or Not Found" });

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                PictureUrl = dto.PictureUrl,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId,
            };

            _context.products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new {id = product.Id}, dto);
        }

        // Delete: api/product/5

        [HttpDelete]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
