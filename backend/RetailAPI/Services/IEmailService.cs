using RetailAPI.Models;

namespace RetailAPI.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmail(User user, Order order, List<OrderItem> items);
    }
}
