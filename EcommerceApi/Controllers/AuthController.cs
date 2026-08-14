using EcommerceApi.DTOs;
using EcommerceApi.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceApi.Constants;

namespace EcommerceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // Post: api/auth/register

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return BadRequest(new AuthResponseDto { Message = "This email is already registered!" });

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AppRoles.Customer);
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { Message = errors});
            }

            var token = await GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "The account has been successfully created!",
                Email = user.Email,
                Username = user.FullName,
                Token = token,
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            });
        }

        // Post: api/auth/login

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized(new AuthResponseDto { Message = "Incorrect email or password!" });

            var token = await GenerateJwtToken(user);

            return Ok(new AuthResponseDto
            {
                IsAuthenticated = true,
                Message = "Logged in successfully!",
                Email = user.Email,
                Username = user.FullName,
                Token = token,
                ExpiresOn = DateTime.UtcNow.AddDays(7)
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var authClamis = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClamis.Add(new Claim(ClaimTypes.Role, role));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["Jwt:DurationInDays"]!)),
                claims: authClamis,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }
}
