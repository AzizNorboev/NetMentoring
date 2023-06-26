using CatalogServiceAPI.Models;
using System.Collections.Generic;

namespace CatalogServiceAPI.ApiModels
{
    public class ItemGetResult
    {
        public int Page { get; }

        public int ItemsOnPage { get; }

        public List<ItemGet> Items { get; }

        public ItemGetResult(int page, int itemsOnPage, List<ItemGet> items)
        {
            Page = page;
            ItemsOnPage = itemsOnPage;
            Items = items;
        }
    }
}
