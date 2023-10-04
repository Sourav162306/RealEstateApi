using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Models;
using RealEstateApi.Data;
using System.Security.Claims;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public PropertiesController(ApiDbContext db)
        {
            _dbContext = db;
        }


        [HttpGet("PropertyList")]
        [Authorize]
        public IActionResult GetCategories(int categoryId)
        {
            var propertiesResult = _dbContext.Properties.Where(c => c.CategoryId == categoryId);
            if(propertiesResult == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(propertiesResult);
            }
        }

        [HttpGet("AllPropertyList")]
        [Authorize]
        public IActionResult GetProperties()
        {
            return Ok(_dbContext.Properties);
        }

        [HttpGet("[action]/{id}")]
        [Authorize]
        public IActionResult PropertyDetail(int id)
        {
            var propertiesResult = _dbContext.Properties.FirstOrDefault(c => c.Id == id);
            if (propertiesResult == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(propertiesResult);
            }
        }

        [HttpGet("TrendingProperties")]
        [Authorize]
        public IActionResult GetTrendingProperties()
        {
            var propertiesResult = _dbContext.Properties.Where(c => c.IsTrending == true);
            if (propertiesResult == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(propertiesResult);
            }
        }

        [HttpGet("SearchProperties")]
        [Authorize]
        public IActionResult GetSearchProperties(string address)
        {
            var propertiesResult = _dbContext.Properties.Where(c => c.Address.Contains(address));
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
        public IActionResult Add([FromBody] Property property)
        {
            if (property == null)
            {
                return NoContent();
            }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _dbContext.Users.First(u => u.Email == userEmail);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    property.UserId = user.Id;
                    _dbContext.Properties.Add(property);
                    _dbContext.SaveChanges();
                    return StatusCode(StatusCodes.Status201Created, "Property Added");
                }
            }
        }

        [HttpPut("[action]/{id}")]
        [Authorize]
        public IActionResult Edit(int id, [FromBody] Property property)
        {
            var propertyResult = _dbContext.Properties.FirstOrDefault(i => i.Id == id);

            if (propertyResult == null)
            {
                return NotFound("Property Not Found");
            }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _dbContext.Users.First(u => u.Email == userEmail);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    if(propertyResult.UserId == user.Id)
                    {
                        propertyResult.Name = property.Name;
                        propertyResult.Detail = property.Detail;
                        propertyResult.Address = property.Address;
                        propertyResult.Price = property.Price;
                        propertyResult.ImageUrl = property.ImageUrl;
                        propertyResult.CategoryId = property.CategoryId;
                        propertyResult.IsTrending = property.IsTrending;

                        _dbContext.SaveChanges();
                        return Ok("Record Updated Successfully");
                    }
                    else
                    {
                        return BadRequest("You Are Not Authorize To Update This Property, Please Provide Valid Token");
                    }
              
                }
            }
        }

        [HttpDelete("[action]/{id}")]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var propertyResult = _dbContext.Properties.FirstOrDefault(i => i.Id == id);

            if (propertyResult == null)
            {
                return NotFound("Property Not Found");
            }
            else
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var user = _dbContext.Users.First(u => u.Email == userEmail);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    if (propertyResult.UserId == user.Id)
                    {
                        _dbContext.Properties.Remove(propertyResult);
                        _dbContext.SaveChanges();
                        return Ok("Record Deleted Successfully");
                    }
                    else
                    {
                        return BadRequest("You Are Not Authorize To Delete This Property, Please Provide Valid Token");
                    }

                }
            }
        }

    }
}
