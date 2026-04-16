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

            return Ok(product);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _dbContext.Categories.ToListAsync());
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            return Ok(await _dbContext.Brands.ToListAsync());
        }
    }
}
