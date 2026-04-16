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
    public class OrdersController : ControllerBase
    {
        private readonly RetailDbContext _dbContext;

        public OrdersController(RetailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var userId = GetUserId();
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Cart is empty.");
            }

            decimal totalAmount = cart.CartItems.Sum(ci => ci.TotalPrice);
            decimal discountApplied = 0; // Implement logic for Coupons if provided

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                DeliveryAddress = request.DeliveryAddress,
                OrderStatus = "Pending",
                PaymentStatus = request.PaymentMethod == "COD" ? "Pending" : "Completed"
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync(); // Save to generate OrderId
            
            // Generate Order Items and handle Inventory Transactions
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice
                };
                
                await _dbContext.OrderItems.AddAsync(orderItem);

                // Deduct stock
                if (item.Product != null)
                {
                    item.Product.StockQuantity -= item.Quantity;
                    
                    var transaction = new InventoryTransaction
                    {
                        ProductId = item.ProductId,
                        QuantityChanged = -item.Quantity,
                        TransactionType = "ORDER_PLACED",
                        Remarks = $"Order #{order.OrderId}"
                    };
                    await _dbContext.InventoryTransactions.AddAsync(transaction);
                }
            }

            // Clear Cart
            _dbContext.CartItems.RemoveRange(cart.CartItems);
            
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Order placed successfully", OrderId = order.OrderId });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = GetUserId();
            var orders = await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }
    }

    public class CheckoutRequest
    {
        public string DeliveryAddress { get; set; } = null!;
        public string PaymentMethod { get; set; } = "COD"; // Defaulting to Cash on Delivery for simplicity
        public string? CouponCode { get; set; }
    }
}
