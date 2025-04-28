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
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Technology> Technologies { get; set; }
        public DbSet<ProductTechnology> ProductTechnologies { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<UserWarehouse> UserWarehouses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Config 1 User => 1 Basket
            builder.Entity<User>()
                .HasOne(u => u.Basket)
                .WithOne(u => u.User)
                .HasForeignKey<Basket>(b => b.UserId);

            // Seed Users
            var seedUsers = new List<User>()
            {
                new User
                {
                    Id = 1,
                    UserName = "spadmin",
                    NormalizedUserName = "SPADMIN",
                    FullName = "SuperAdmin",
                    Email = "spadmin@example.com",
                    NormalizedEmail = "SPADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = 2,
                    UserName = "admin1",
                    FullName = "Admin1",
                    NormalizedUserName = "ADMIN1",
                    Email = "admin1@example.com",
                    NormalizedEmail = "ADMIN1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = 3,
                    UserName = "admin2",
                    FullName = "Admin2",
                    NormalizedUserName = "ADMIN2",
                    Email = "admin2@example.com",
                    NormalizedEmail = "ADMIN2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new User
                {
                    Id = 4,
                    UserName = "admin3",
                    FullName = "Admin3",
                    NormalizedUserName = "ADMIN3",
                    Email = "admi3n@example.com",
                    NormalizedEmail = "ADMIN3@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            // Hash mật khẩu
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            seedUsers.ForEach(u => u.PasswordHash = hasher.HashPassword(u, $"{u.UserName}@123"));
            builder.Entity<User>().HasData(seedUsers);

            // Seed Role Data
            builder.Entity<Role>()
                .HasData(
                    new { Id = 1, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                    new { Id = 2, Name = "Admin", NormalizedName = "ADMIN" },
                    new { Id = 3, Name = "Customer", NormalizedName = "CUSTOMER" }
            );

            // Seed User - Role
            var userUserRoles = new List<IdentityUserRole<int>>
            {
                new IdentityUserRole<int>
                {
                    UserId = 1,
                    RoleId = 1
                },
                new IdentityUserRole<int>
                {
                    UserId = 2,
                    RoleId = 2
                },
                new IdentityUserRole<int>
                {
                    UserId = 3,
                    RoleId = 2
                },
                new IdentityUserRole<int>
                {
                    UserId = 4,
                    RoleId = 2
                },
            };
            builder.Entity<IdentityUserRole<int>>().HasData(userUserRoles);

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

            // Config Order Owned ShippingAddress
            builder.Entity<Order>()
                .OwnsOne(o => o.ShippingAddress, sa =>
                {
                    sa.WithOwner();
                    sa.Property(p => p.City).HasColumnName("City");
                    sa.Property(p => p.District).HasColumnName("District");
                    sa.Property(p => p.Ward).HasColumnName("Ward");
                    sa.Property(p => p.StreetAddress).HasColumnName("StreetAddress");
                    sa.Property(p => p.PostalCode).HasColumnName("PostalCode");
                    sa.Property(p => p.Country).HasColumnName("Country");
                })
                .Navigation(x => x.ShippingAddress)
                .IsRequired();

            // Config Order Item Owned ProductOrderItem
            builder.Entity<OrderItem>()
                .OwnsOne(o => o.ProductOrderItem, sa =>
                {
                    sa.WithOwner();
                    sa.Property(p => p.ProductDetailId).HasColumnName("ProductDetailId");
                    sa.Property(p => p.ProductName).HasColumnName("ProductName");
                    sa.Property(p => p.ProductImageUrl).HasColumnName("ProductImageUrl");
                    sa.Property(p => p.ProductPrice).HasColumnName("ProductPrice");
                })
                .Navigation(x => x.ProductOrderItem)
                .IsRequired();

            // Config UserAddress Owned ShippingAddress
            builder.Entity<UserAddress>()
                .OwnsOne(o => o.ShippingAddress, sa =>
                {
                    sa.WithOwner();
                    sa.Property(p => p.City).HasColumnName("City");
                    sa.Property(p => p.District).HasColumnName("District");
                    sa.Property(p => p.Ward).HasColumnName("Ward");
                    sa.Property(p => p.StreetAddress).HasColumnName("StreetAddress");
                    sa.Property(p => p.PostalCode).HasColumnName("PostalCode");
                    sa.Property(p => p.Country).HasColumnName("Country");
                })
                .Navigation(x => x.ShippingAddress)
                .IsRequired();
        }
    }
}
