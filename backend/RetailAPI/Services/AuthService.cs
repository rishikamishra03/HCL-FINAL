using Microsoft.IdentityModel.Tokens;
using RetailAPI.Data;
using RetailAPI.DTOs;
using RetailAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RetailAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<bool> RegisterAsync(RegisterRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly RetailDbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthService(RetailDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Simplified plain text password for demo, typically use BCrypt here.
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == request.Password);
            if (user == null) return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponse
            {
                Token = tokenHandler.WriteToken(token),
                FullName = user.FullName,
                Role = user.Role
            };
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Email == request.Email)) return false;

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password, // Demo purposes: store hashed password in prod
                Role = "Customer"
            };

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
