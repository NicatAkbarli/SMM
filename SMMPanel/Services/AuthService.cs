using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SMMPanel.Data;
using SMMPanel.Dtos;
using SMMPanel.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SMMPanel.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService; // ✅ əlavə et

        public AuthService(AppDbContext context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        public async Task<string> Register(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return "İstifadəçi artıq mövcuddur";

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return "Email artıq istifadə olunur";

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // təsdiqləmə kodu
            var code = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User",
                EmailConfirmed = false,
                VerificationCode = code,
                VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // ✅ kodu email ilə göndər
            await _emailService.SendVerificationEmailAsync(user.Email, code);

            return "Qeydiyyat uğurla başa çatdı. Zəhmət olmasa email-ə gələn kodu təsdiqləyin.";
        }

        public async Task<(string token, string refreshToken)> Login(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return (null, null);

            if (!user.EmailConfirmed)
                return (null, null); // Email təsdiqlənməyibsə login etmirik

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

            await _context.SaveChangesAsync();

            return (token, refreshToken);
        }

        public async Task<string> ConfirmEmail(string email, string code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return "İstifadəçi tapılmadı";

            if (user.EmailConfirmed) return "Email artıq təsdiqlənib";

            if (user.VerificationCode != code || user.VerificationCodeExpiry < DateTime.UtcNow)
                return "Kod səhvdir və ya vaxtı bitib";

            user.EmailConfirmed = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            await _context.SaveChangesAsync();

            return "Email uğurla təsdiqləndi ✅";
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
