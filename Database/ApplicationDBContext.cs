using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public DbSet<Stock> Stocks { get; set; }
    }
}