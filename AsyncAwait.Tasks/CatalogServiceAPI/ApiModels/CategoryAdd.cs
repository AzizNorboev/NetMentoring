using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace CatalogServiceAPI.Models
{
    public class CategoryAdd
    {
        public string Name { get; set;  }

        public string Description { get; set; }
    }
}
