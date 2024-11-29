using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store_API.Models;
using Store_API.Models.Order;

namespace Store_API.Data
{
    public class StoreContext : IdentityDbContext<User, Role, int>
    {
        public StoreContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Althete> Althetes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Config 1 User => 1 Basket
            builder.Entity<User>()
                .HasOne(u => u.Basket)
                .WithOne(u => u.User)
                .HasForeignKey<Basket>(b => b.UserId);

            // Seed Role Data
            builder.Entity<Role>()
                .HasData(
                    new { Id = 1, Name = "Manager", NormalizedName = "MANAGER" },
                    new { Id = 2, Name = "Admin", NormalizedName = "ADMIN" },
                    new { Id = 3, Name = "Customer", NormalizedName = "CUSTOMER" }
            );
        }
    }
}
