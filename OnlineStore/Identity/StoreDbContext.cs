using Microsoft.EntityFrameworkCore;
using OnlineStore.Entities;

namespace OnlineStore.Identity
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
            
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

    }
}
