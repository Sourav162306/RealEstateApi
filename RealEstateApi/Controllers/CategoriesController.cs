using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateApi.Data;
using RealEstateApi.Models;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public CategoriesController(ApiDbContext db)
        {
            _dbContext = db;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_dbContext.Categories.Include("Properties"));
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public IActionResult CategoryDetail(int id)
        {
            var categoryResult = _dbContext.Categories.Include("Properties").FirstOrDefault(c => c.Id == id);
            if (categoryResult == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(categoryResult);
            }
        }

        [HttpGet("SearchCategories")]
        [Authorize]
        public IActionResult GetSearchProperties(string name)
        {
            var propertiesResult = _dbContext.Categories.Include("Properties").Where(c => c.Name.Contains(name));
            if (propertiesResult == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(propertiesResult);
            }
        }

        [HttpPost("[action]")]
        [Authorize]
        public IActionResult Add([FromBody] Category category)
        {
            var categoryExists = _dbContext.Categories.FirstOrDefault(u => u.Name == category.Name);

            if (categoryExists != null)
            {
                return BadRequest("Category With Same Name Exists");
            }

            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, "Category Item Created");
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public IActionResult Edit(int id, [FromBody] Category category)
        {
            var categoryResult = _dbContext.Categories.FirstOrDefault(i => i.Id == id);

            if (categoryResult == null)
            {
                return NotFound("Category Not Found");
            }
            else
            {
                categoryResult.Name = category.Name;
                categoryResult.ImageUrl = category.ImageUrl;
                _dbContext.SaveChanges();
                return Ok("Category Item Updated Successfully");
            }
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var categoryResult = _dbContext.Categories.FirstOrDefault(i => i.Id == id);
            if (categoryResult == null)
            {
                return NotFound("Category Item Not Found");
            }
            _dbContext.Categories.Remove(categoryResult);
            _dbContext.SaveChanges();
            return Ok("Category Deleted Successfully");

        }

    }
}
