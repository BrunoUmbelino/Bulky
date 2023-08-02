
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
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
                new Category() { Id = 1, DisplayOrder = 1, Name = "Ficção Científica" },
                new Category() { Id = 2, DisplayOrder = 2, Name = "Terror" },
                new Category() { Id = 3, DisplayOrder = 3, Name = "Ação e aventura" }
                );
        }
    }
}
