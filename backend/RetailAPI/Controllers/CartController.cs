using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailAPI.Data;
using RetailAPI.Models;
using System.Security.Claims;

namespace RetailAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly RetailDbContext _dbContext;

        public CartController(RetailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserId();
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            // Apply Smart Image Mapping to Cart Items
            foreach (var item in cart.CartItems)
            {
                if (item.Product != null && string.IsNullOrEmpty(item.Product.ImageUrl))
                {
                    var catName = item.Product.Category?.CategoryName?.Trim().ToLower() ?? "";
                    item.Product.ImageUrl = catName switch
                    {
                        var s when s.Contains("pizza") => "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=500",
                        var s when s.Contains("drink") || s.Contains("coke") || s.Contains("cola") => "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500",
                        var s when s.Contains("bread") || s.Contains("garlic") => "https://images.unsplash.com/photo-1573140247632-f8fd73ad6744?w=500",
                        _ => "https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=500"
                    };
                }
            }

            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemRequest request)
        {
            var userId = GetUserId();
            var cart = await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
            
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _dbContext.Carts.Add(cart);
                await _dbContext.SaveChangesAsync();
            }

            var product = await _dbContext.Products.FindAsync(request.ProductId);
            if (product == null) return NotFound("Product not found");

            var existingItem = await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = product.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.BasePrice,
                    TotalPrice = product.BasePrice * request.Quantity
                };
                _dbContext.CartItems.Add(newItem);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new { Message = "Item added to cart" });
        }
    }

    public class CartItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
