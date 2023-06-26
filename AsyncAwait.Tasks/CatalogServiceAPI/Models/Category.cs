using System.Collections.Generic;

namespace CatalogServiceAPI.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public IReadOnlyCollection<Item> Items { get; set; } = new List<Item>();
    }
}
