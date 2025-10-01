using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SMMPanel.Dtos;
using SMMPanel.Services;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SMMPanel.Data;

namespace SMMPanel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;

        public AuthController(IAuthService authService, AppDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto request)
        {
            var result = await _authService.Register(request);

            if (result.Contains("artıq"))
                return BadRequest(new { error = result });

            return Ok(new { message = result });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(string email, string code)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return BadRequest(new { error = "İstifadəçi tapılmadı" });

            if (user.EmailConfirmed)
                return BadRequest(new { error = "Email artıq təsdiqlənib" });

            if (user.VerificationCode != code || user.VerificationCodeExpiry < DateTime.UtcNow)
                return BadRequest(new { error = "Kod yanlışdır və ya vaxtı bitib" });

            user.EmailConfirmed = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Email uğurla təsdiqləndi" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto request)
        {
            var (token, refreshToken) = await _authService.Login(request);

            if (token == null)
                return Unauthorized(new { error = "İstifadəçi adı və ya şifrə yanlışdır" });

            return Ok(new
            {
                token,
                refreshToken
            });
        }

        [Authorize]
        [HttpPost("add-balance")]
        public async Task<IActionResult> AddBalance([FromBody] decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Məbləğ düzgün deyil");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            user.Balance += amount;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Balans artırıldı", balance = user.Balance });
        }

        [Authorize]
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Unauthorized();

            return Ok(new { balance = user.Balance });
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var username = User.Identity?.Name;
            return Ok(new { message = $"Salam, {username}! Bu endpoint token ilə qorunur." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnly()
        {
            return Ok(new { message = "Yalnız adminlər daxil ola bilər" });
        }
    }
}
