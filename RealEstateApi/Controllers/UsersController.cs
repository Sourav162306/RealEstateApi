﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateApi.Data;
using RealEstateApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RealEstateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;
        private readonly IConfiguration _config;

        public UsersController(ApiDbContext db, IConfiguration config)
        {
            _dbContext = db;
            _config = config;
        }

        [HttpGet("Email")]
        public IActionResult GetUser(string email)
        {
            var userResult = _dbContext.Users.FirstOrDefault(c => c.Email == email);
            if (userResult == null)
            {
                return NotFound("User Not Found! Please Provide Correct Email");
            }
            else
            {
                return Ok(userResult);
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_dbContext.Users);
        }

        [HttpPost("[action]")]
        public IActionResult Register([FromBody] User user)
        {
            var userExists = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

            if (userExists != null)
            {
                return BadRequest("User With Same Email Exists");
            }

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created, "User Created");
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody] User user)
        {
            var currentUser = _dbContext.Users.FirstOrDefault(u => u.Email == user.Email);

            if (currentUser == null)
            {
                return NotFound("User With This Email Doesn't Exist");
            }
            else
            {
                if(currentUser.Password == user.Password)
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                    var claims = new[]
                        {
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                    var token = new JwtSecurityToken(
                        issuer: _config["JWT:Issuer"],
                        audience: _config["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(600),
                        signingCredentials: credentials
                    );
                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(jwt);
                }
                else
                {
                    return BadRequest("Password Incorrect");
                }
            }
            
        }
    }
}
