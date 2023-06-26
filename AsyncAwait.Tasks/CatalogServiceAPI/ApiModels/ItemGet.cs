namespace CatalogServiceAPI.Models
{
    public class ItemGet
    {
        public int Id { get; }

        public string Name { get; }

        public CategoryGet Category { get; }

        public ItemGet(int id, string name, CategoryGet category)
        {
            Id = id;
            Name = name;
            Category = category;
        }
    }
}
