using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Data.DbContexts
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<TestProduct> TestProducts { get; set; }
    }
}
