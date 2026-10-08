using Microsoft.EntityFrameworkCore;

namespace EF_Lab.Models
{
    public partial class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) :base(options)
        {
            
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Company> Companies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasCheckConstraint("Ch_Price_Range", $"[Price] >= 0 AND [Price] <= {decimal.MaxValue}");
                }
            );

        }
    }
}
