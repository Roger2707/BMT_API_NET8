using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store_API.Models;
using Store_API.Models.Inventory;
using Store_API.Models.OrderAggregate;
using Store_API.Models.Users;

namespace Store_API.Data
{
    public class StoreContext : IdentityDbContext<User, Role, int>
    {
        public StoreContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<Payment> Payments { get; set; }


        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<ProductTechnology> ProductTechnologies { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Config 1 User => 1 Basket
            builder.Entity<User>()
                .HasOne(u => u.Basket)
                .WithOne(u => u.User)
                .HasForeignKey<Basket>(b => b.UserId);

            // Seed Admin User
            var adminUser = new User
            {
                Id = 1,
                UserName = "admin",
                FullName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // Hash mật khẩu (123456)
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");
            builder.Entity<User>().HasData(adminUser);

            // Seed Role Data
            builder.Entity<Role>()
                .HasData(
                    new { Id = 1, Name = "Manager", NormalizedName = "MANAGER" },
                    new { Id = 2, Name = "Admin", NormalizedName = "ADMIN" },
                    new { Id = 3, Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Seed Admin Role
            var adminUserRole = new IdentityUserRole<int>
            {
                UserId = 1,
                RoleId = 2
            };
            builder.Entity<IdentityUserRole<int>>().HasData(adminUserRole);

            // User - Address (1-N)
            builder.Entity<UserAddress>()
                .HasOne(a => a.User)
                .WithMany(u => u.UserAddresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Order (1-N)
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order - Address (N-1)
            builder.Entity<Order>()
                .HasOne(o => o.UserAddress)
                .WithMany()
                .HasForeignKey(o => o.UserAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Config Relation Product (1 -> n) ProductDetail
            builder.Entity<Product>()
                .HasMany(p => p.Details)
                .WithOne(d => d.Product)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Config Relation Product (n -> n) Technology
            builder.Entity<ProductTechnology>()
                .HasKey(pt => new { pt.ProductId, pt.TechnologyId }); // Thiết lập composite key

            builder.Entity<ProductTechnology>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.Technologies)
                .HasForeignKey(pt => pt.ProductId);

            builder.Entity<ProductTechnology>()
                .HasOne(pt => pt.Technology)
                .WithMany(t => t.Products)
                .HasForeignKey(pt => pt.TechnologyId);
        }
    }
}
