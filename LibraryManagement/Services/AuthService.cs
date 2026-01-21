using LibraryManagement.Data;
using LibraryManagement.DTOs;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagement.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> Login(LoginDto loginDto);
        Task<AuthResponseDto?> Register(RegisterDto registerDto);
        Task<User?> GetUserById(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly LibraryDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LibraryDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            };
        }

        public async Task<AuthResponseDto?> Register(RegisterDto registerDto)
        {
            // Check if user already exists
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                return null;
            }

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = UserRole.Member, // Default role
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                CreatedDate = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                Token = token
            };
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForLibraryManagementSystem12345"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "LibraryManagement",
                audience: _configuration["Jwt:Audience"] ?? "LibraryManagement",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
