
using System;
using System.Threading.Tasks;
using CatalogServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogServiceAPI.Infrastructure
{
    public class CatalogDbContext: DbContext, ICatalogDbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options): base(options)
        {
        }

        public DbSet<Item> Items { get; set; }

        public DbSet<Category> Categories { get; set; }
        public async Task<int> SaveAsync()
        {
            return await SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentNullException(nameof(modelBuilder));

            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);

            modelBuilder.Entity<Item>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Items);

            modelBuilder.Entity<Item>()
                .HasKey(x => x.Id);
            
            modelBuilder.Entity<Category>()
                .HasKey(x => x.Id);
        }
    }
}
