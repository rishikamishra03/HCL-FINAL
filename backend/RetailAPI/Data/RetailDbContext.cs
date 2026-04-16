using Microsoft.EntityFrameworkCore;
using RetailAPI.Models;

namespace RetailAPI.Data
{
    public class RetailDbContext : DbContext
    {
        public RetailDbContext(DbContextOptions<RetailDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Packaging> Packagings { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Coupon> Coupons { get; set; } = null!;
        public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;
        public DbSet<OrderCoupon> OrderCoupons { get; set; } = null!;
        public DbSet<EmailNotification> EmailNotifications { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Decimal configurations
            modelBuilder.Entity<Packaging>()
                .Property(p => p.ExtraPrice)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<Product>()
                .Property(p => p.BasePrice)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<Coupon>()
                .Property(c => c.DiscountValue)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<CartItem>()
                .Property(c => c.UnitPrice)
                .HasColumnType("DECIMAL(10, 2)");
            modelBuilder.Entity<CartItem>()
                .Property(c => c.TotalPrice)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<OrderItem>()
                .Property(o => o.UnitPrice)
                .HasColumnType("DECIMAL(10, 2)");
            modelBuilder.Entity<OrderItem>()
                .Property(o => o.TotalPrice)
                .HasColumnType("DECIMAL(10, 2)");
                
            modelBuilder.Entity<OrderCoupon>()
                .Property(oc => oc.DiscountApplied)
                .HasColumnType("DECIMAL(10, 2)");

            // Relationships
            // Users -> LoyaltyAccount 1:1
            modelBuilder.Entity<User>()
                .HasOne(u => u.LoyaltyAccount)
                .WithOne(l => l.User)
                .HasForeignKey<LoyaltyAccount>(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure other cascading behaviors matching the script if necessary
            // For example, OrderItem -> Order is CASCADE, OrderItem -> Product is RESTRICT
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
