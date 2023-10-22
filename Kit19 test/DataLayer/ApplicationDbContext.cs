using Kit19_test.Models;
using Microsoft.EntityFrameworkCore;

namespace Kit19_test.DataLayer
{

    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18, 0)");        }
    }

}