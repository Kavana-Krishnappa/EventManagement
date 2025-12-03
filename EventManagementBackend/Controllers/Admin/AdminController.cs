using BCrypt.Net;
using EventManagement.DTOs;
using EventManagement.Models;
using EventManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EventManagement.Controllers.Admin
{
    [ApiController]
    [Route("api/auth")]
    public class AdminController : ControllerBase
    {

        //DI
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;

        public AdminController(IConfiguration configuration, ILogger<AdminController> logger, ApplicationDbContext context, ITokenService tokenService)
        {
            _configuration = configuration;
            _logger = logger;
            _dbContext = context;
            _tokenService = tokenService;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AdminCreateDTO adminCreateDTO)
        {
            if (await _dbContext.Admins
    .AnyAsync(a => a.Email.ToLower() == adminCreateDTO.Email.ToLower()))
            {
                return BadRequest("Email already in use.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(adminCreateDTO.Password);

            var admin = new Models.Admin
            {
                FullName = adminCreateDTO.FullName,
                Email = adminCreateDTO.Email,
                PasswordHash = passwordHash,
                Role = adminCreateDTO.Role,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Admins.Add(admin);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Admin registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginDTO adminLoginDTO)
        {
            var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Email == adminLoginDTO.Email);
           
            if (admin == null || !BCrypt.Net.BCrypt.Verify(adminLoginDTO.Password, admin.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var token = _tokenService.GenerateToken(admin);

            
            return Ok(new
            {
                token,
                admin = new
                {
                    admin.AdminId,
                    admin.FullName,
                    admin.Email,
                    admin.Role
                }
            });
        }

        private bool VerifyPasswordHash(string plainPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
        }



        
    }
}
