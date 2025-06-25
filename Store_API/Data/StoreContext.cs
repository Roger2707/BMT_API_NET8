using MassTransit;
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
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
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
        public DbSet<ShippingOrder> ShippingOrders { get; set; }
        public DbSet<StockHold> StockHolds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Seeding Users - Roles - UserRoles

            var users = DBNewSeed.SeedUsersData();
            var roles = DBNewSeed.SeedRoleData();
            var userRoles = DBNewSeed.SeedUserRoleRelations();

            builder.Entity<User>().HasData(users);
            builder.Entity<IdentityUserRole<int>>().HasData(userRoles);
            builder.Entity<Role>().HasData(roles);

            #endregion

            #region Seeding Categories - Brands - Promotions

            var categories = DBNewSeed.SeedCategories();
            var brands = DBNewSeed.SeedBrands();
            var promotions = DBNewSeed.SeedPromotions();

            builder.Entity<Category>().HasData(categories);
            builder.Entity<Brand>().HasData(brands);
            builder.Entity<Promotion>().HasData(promotions);

            #endregion

            #region Products - ProductDetails - ProductTechnologies - Technologies - Ratings

            var products = DBNewSeed.SeedProducts();
            var technologies = DBNewSeed.SeedTechnologies();
            var productTechnologies = DBNewSeed.SeedProductTechnologies();
            var productDetails = DBNewSeed.SeedProductDetails();
            var ratings = DBNewSeed.SeedRatings();

            builder.Entity<Product>().HasData(products);
            builder.Entity<Technology>().HasData(technologies);
            builder.Entity<ProductTechnology>().HasData(productTechnologies);
            builder.Entity<ProductDetail>().HasData(productDetails);
            builder.Entity<Rating>().HasData(ratings);

            #endregion

            #region Seed Warehouse - Stocks - StockTransactions - UserWarehouses

            var warehouses = DBNewSeed.SeedWarehouses();
            var stocks = DBNewSeed.SeedStocks();
            var stockTransactions = DBNewSeed.SeedStockTransactions();
            var userWarehouses = DBNewSeed.SeedUserWarehouses();

            builder.Entity<Warehouse>().HasData(warehouses);
            builder.Entity<Stock>().HasData(stocks);
            builder.Entity<StockTransaction>().HasData(stockTransactions);
            builder.Entity<UserWarehouse>().HasData(userWarehouses);

            #endregion

            #region Config Relations 

            // Config 1 User => 1 Basket
            builder.Entity<User>()
                .HasOne(u => u.Basket)
                .WithOne(u => u.User)
                .HasForeignKey<Basket>(b => b.UserId);

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

            #endregion

            #region Outbox Pattern

            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();

            #endregion
        }
    }
}
