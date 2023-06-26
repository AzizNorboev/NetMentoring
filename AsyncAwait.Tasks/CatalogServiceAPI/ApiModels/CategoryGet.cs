namespace CatalogServiceAPI.Models
{
    public class CategoryGet
    {
        public int Id { get; }

        public string Name { get; }

        public string Description { get; }

        public CategoryGet()
        {
            
        }

        public CategoryGet(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
