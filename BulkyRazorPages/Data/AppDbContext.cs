using BulkyRazorPages.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyRazorPages.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                    new Category() { Id = 1, Name = "Terror", DisplayOrder = 1 },
                    new Category() { Id = 2, Name = "Fantasia", DisplayOrder = 2 },
                    new Category() { Id = 3, Name = "Documentário", DisplayOrder = 3 }
                );
        }
    }
}
