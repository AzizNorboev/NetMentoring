using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServiceAPI.Infrastructure;
using CatalogServiceAPI.Models;

namespace CatalogServiceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICatalogDbContext _dbContext;

        public CategoriesController(ICatalogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoryGet>> GetCategories()
        {
            var categories = _dbContext.Categories.Select(x => new CategoryGet(x.Id, x.Name, x.Description));

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryAdd category)
        {
            var existingCategories = _dbContext.Categories.Select(x => new CategoryGet(x.Id, x.Name, x.Description));
            if (existingCategories.Any(x => x.Name == category.Name))
            {
                return BadRequest("Category with this name already exists");
            }

            _dbContext.Categories.Add(new Category()
            {
                Name = category.Name,
                Description = category.Description
            });
            await _dbContext.SaveAsync();

            return Ok();

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryUpdate categoryUpdateDto)
        {
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound("Category does not exist");
            }

            category.Name = categoryUpdateDto.Name;
            category.Description = categoryUpdateDto.Description;

            await _dbContext.SaveAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                return NotFound("Category does not exist");
            }

            var categoryItems = _dbContext.Items.Where(x => x.CategoryId == id);

            _dbContext.Items.RemoveRange(categoryItems);

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveAsync();

            return Ok();
        }
    }
}
