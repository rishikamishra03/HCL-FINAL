using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RetailAPI.Models
{
    [Table("CATEGORY")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("BRAND")]
    public class Brand
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("PACKAGING")]
    public class Packaging
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PackagingId { get; set; }
        public string PackagingType { get; set; } = null!;
        public string? Size { get; set; }
        public decimal ExtraPrice { get; set; } = 0.00m;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    [Table("USER")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Role { get; set; } = "Customer";
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public LoyaltyAccount? LoyaltyAccount { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    [Table("COUPON")]
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponId { get; set; }
        public string CouponCode { get; set; } = null!;
        public string DiscountType { get; set; } = null!;
        public decimal DiscountValue { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    [Table("PRODUCT")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        [NotMapped]
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? PackagingId { get; set; }
        public int StockQuantity { get; set; } = 0;
        public bool IsAvailable { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        [ForeignKey("BrandId")]
        public Brand? Brand { get; set; }
        [ForeignKey("PackagingId")]
        public Packaging? Packaging { get; set; }
    }

    [Table("CART")]
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    [Table("ORDER")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = null!;
        public string? DeliveryAddress { get; set; }
        public string PaymentStatus { get; set; } = null!;

        [ForeignKey("UserId")]
        public User? User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    [Table("LOYALTY_ACCOUNT")]
    public class LoyaltyAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoyaltyAccountId { get; set; }
        public int? UserId { get; set; }
        public int PointBalance { get; set; } = 0;
        public DateTime? LastUpdated { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }
    }

    [Table("CART_ITEM")]
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemId { get; set; }
        public int? CartId { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        [ForeignKey("CartId")]
        public Cart? Cart { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }

    [Table("ORDER_ITEM")]
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }

    [Table("INVENTORY_TRANSACTION")]
    public class InventoryTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public int? ProductId { get; set; }
        public int QuantityChanged { get; set; }
        public string TransactionType { get; set; } = null!;
        public DateTime? TransactionDate { get; set; } = DateTime.UtcNow;
        public string? Remarks { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }

    [Table("ORDER_COUPON")]
    public class OrderCoupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderCouponId { get; set; }
        public int? OrderId { get; set; }
        public int? CouponId { get; set; }
        public decimal DiscountApplied { get; set; }

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        [ForeignKey("CouponId")]
        public Coupon? Coupon { get; set; }
    }

    [Table("EMAIL_NOTIFICATION")]
    public class EmailNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailNotificationId { get; set; }
        public int? OrderId { get; set; }
        public int? UserId { get; set; }
        public string EmailType { get; set; } = null!;
        public DateTime? SentAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = null!;

        [ForeignKey("OrderId")]
        public Order? Order { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
