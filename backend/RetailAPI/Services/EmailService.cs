using RetailAPI.Models;
using System.Text;

namespace RetailAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmail(User user, Order order, List<OrderItem> items)
        {
            var emailFolder = Path.Combine(Directory.GetCurrentDirectory(), "emails");
            if (!Directory.Exists(emailFolder)) Directory.CreateDirectory(emailFolder);

            var filePath = Path.Combine(emailFolder, $"Order_{order.OrderId}.html");

            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>");
            sb.AppendLine("<div style='max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 15px rgba(0,0,0,0.1);'>");
            sb.AppendLine("<h1 style='color: #f43f5e;'>Order Confirmed! 🍕</h1>");
            sb.AppendLine($"<p>Hello <strong>{user.FullName}</strong>,</p>");
            sb.AppendLine("<p>Thank you for your order! Your payment has been confirmed and we are preparing your delicious items.</p>");
            sb.AppendLine("<hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>");
            sb.AppendLine($"<p><strong>Order ID:</strong> #{order.OrderId}</p>");
            sb.AppendLine($"<p><strong>Shipping To:</strong> {order.DeliveryAddress}</p>");
            sb.AppendLine("<h3>Items Ordered:</h3>");
            sb.AppendLine("<table style='width: 100%; border-collapse: collapse;'>");
            sb.AppendLine("<tr style='background: #f8f8f8;'><th style='padding: 10px; text-align: left;'>Product</th><th style='padding: 10px;'>Qty</th><th style='padding: 10px; text-align: right;'>Total</th></tr>");

            foreach (var item in items)
            {
                sb.AppendLine($"<tr><td style='padding: 10px; border-bottom: 1px solid #eee;'>{item.Product?.ProductName ?? "Item"}</td><td style='padding: 10px; border-bottom: 1px solid #eee; text-align: center;'>{item.Quantity}</td><td style='padding: 10px; border-bottom: 1px solid #eee; text-align: right;'>${item.TotalPrice:F2}</td></tr>");
            }

            sb.AppendLine("</table>");
            sb.AppendLine($"<h2 style='text-align: right; color: #10b981;'>Grand Total: ${order.TotalAmount:F2}</h2>");
            sb.AppendLine("<p style='font-size: 12px; color: #777; margin-top: 30px;'>Sent to: " + user.Email + "</p>");
            sb.AppendLine("</div></body></html>");

            await File.WriteAllTextAsync(filePath, sb.ToString());

            // Also keep the console log for easy monitoring
            _logger.LogInformation($"📧 HTML Email generated for Order #{order.OrderId} at: {filePath}");
        }
    }
}
