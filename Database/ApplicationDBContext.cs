
using EShopBE.models;
using Microsoft.EntityFrameworkCore;

namespace EShopBE.Database
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
: base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Tắt logging SQL
            optionsBuilder
                .UseLoggerFactory(null)  // Tắt logging
                .EnableSensitiveDataLogging(false); // Tắt logging nhạy cảm
        }
        public DbSet<Product> Products { get; set; }
    }
}