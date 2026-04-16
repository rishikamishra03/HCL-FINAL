using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly RetailDbContext _dbContext;

        public ProductsController(RetailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] int? categoryId, [FromQuery] int? brandId)
        {
            var query = _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Packaging)
                .Where(p => p.IsAvailable);

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId.Value);
            }

            var products = await query.ToListAsync();

            // Smart Image Mapping: Ensure Pizza shows Pizza, etc.
            foreach (var p in products)
            {
                if (string.IsNullOrEmpty(p.ImageUrl))
                {
                    var catName = p.Category?.CategoryName?.Trim().ToLower() ?? "";
                    p.ImageUrl = catName switch
                    {
                        var s when s.Contains("pizza") => "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=500",
                        var s when s.Contains("drink") || s.Contains("coke") || s.Contains("cola") => "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500",
                        var s when s.Contains("bread") || s.Contains("garlic") => "https://images.unsplash.com/photo-1573140247632-f8fd73ad6744?w=500",
                        _ => "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500"
                    };
                }
            }

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Packaging)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null) return NotFound();

            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                var catName = product.Category?.CategoryName?.Trim().ToLower() ?? "";
                product.ImageUrl = catName switch
                {
                    var s when s.Contains("pizza") => "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=500",
                    var s when s.Contains("drink") || s.Contains("coke") || s.Contains("cola") => "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500",
                    var s when s.Contains("bread") || s.Contains("garlic") => "https://images.unsplash.com/photo-1573140247632-f8fd73ad6744?w=500",
                    _ => "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500"
                };
            }

            return Ok(product);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            var uniqueCategories = categories
                .GroupBy(c => c.CategoryName.Trim().ToLower())
                .Select(g => g.First())
                .ToList();
            return Ok(uniqueCategories);
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            return Ok(await _dbContext.Brands.ToListAsync());
        }
    }
}
