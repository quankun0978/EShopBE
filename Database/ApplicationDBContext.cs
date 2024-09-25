
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
        public DbSet<Product> Products { get; set; }
    }
}