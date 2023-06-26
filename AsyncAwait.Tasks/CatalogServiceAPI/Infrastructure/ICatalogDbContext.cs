using System.Threading.Tasks;
using CatalogServiceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogServiceAPI.Infrastructure
{
    public interface ICatalogDbContext
    {
        public DbSet<Item> Items { get; set; }

        public DbSet<Category> Categories { get; set; }

        public Task<int> SaveAsync();
    }
}
