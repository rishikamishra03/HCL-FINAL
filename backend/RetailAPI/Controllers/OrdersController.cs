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
        private readonly IEmailService _emailService;

        public OrdersController(RetailDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
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
            decimal discountApplied = 0;
            Coupon? coupon = null;

            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                // Check if user has at least 500 loyalty points to be eligible for discounts
                var loyaltyAccount = await _dbContext.LoyaltyAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
                var userPoints = loyaltyAccount?.PointBalance ?? 0;

                if (userPoints >= 500)
                {
                    coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == request.CouponCode && c.IsActive && c.ExpiryDate > DateTime.Now);
                    if (coupon != null)
                    {
                        discountApplied = coupon.DiscountType == "Percentage" 
                            ? (totalAmount * (coupon.DiscountValue / 100)) 
                            : coupon.DiscountValue;
                    }
                }
                else
                {
                    // Optionally, we could return a BadRequest if they try to use a coupon without enough points,
                    // but for now we just don't apply the discount.
                    coupon = null; 
                }
            }

            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount - discountApplied,
                DeliveryAddress = request.DeliveryAddress,
                OrderStatus = "Confirmed",
                PaymentStatus = request.PaymentMethod == "COD" ? "Pending" : "Completed"
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync(); 

            if (coupon != null)
            {
                await _dbContext.OrderCoupons.AddAsync(new OrderCoupon { 
                    OrderId = order.OrderId, 
                    CouponId = coupon.CouponId, 
                    DiscountApplied = discountApplied 
                });
            }
            
            // Add Loyalty Points (10 points per dollar spent)
            var loyalty = await _dbContext.LoyaltyAccounts.FirstOrDefaultAsync(l => l.UserId == userId);
            if (loyalty == null)
            {
                loyalty = new LoyaltyAccount { UserId = userId, PointBalance = 0 };
                _dbContext.LoyaltyAccounts.Add(loyalty);
            }
            loyalty.PointBalance += (int)(order.TotalAmount * 10);

            // Create Email Notification Record
            var emailNotification = new EmailNotification {
                OrderId = order.OrderId,
                UserId = userId,
                EmailType = "ORDER_CONFIRMATION",
                Status = "QUEUED"
            };
            await _dbContext.EmailNotifications.AddAsync(emailNotification);
            
            // Generate Order Items and handle Inventory
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

            await _dbContext.SaveChangesAsync();

            // Trigger Email Simulation
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                var orderItems = await _dbContext.OrderItems
                    .Include(oi => oi.Product)
                    .Where(oi => oi.OrderId == order.OrderId)
                    .ToListAsync();
                
                await _emailService.SendOrderConfirmationEmail(user, order, orderItems);
                
                emailNotification.Status = "SENT";
                await _dbContext.SaveChangesAsync();
            }

            _dbContext.CartItems.RemoveRange(cart.CartItems);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Order placed successfully! Confirmation email queued.", OrderId = order.OrderId });
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
