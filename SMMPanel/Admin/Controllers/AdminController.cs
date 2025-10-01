using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMMPanel.Data;

namespace SMMPanel.AdminPanel.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeUserRole(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("İstifadəçi tapılmadı");

            if (string.IsNullOrWhiteSpace(newRole) || (newRole != "User" && newRole != "Admin"))
                return BadRequest("Rol yalnız 'User' və ya 'Admin' ola bilər");

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Rol uğurla dəyişdirildi", userId = user.Id, newRole = user.Role });
        }


        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Balance,
                    u.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("update-balance")]
        public async Task<IActionResult> UpdateBalance(int userId, decimal amount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("İstifadəçi tapılmadı");

            user.Balance = amount;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Balans yeniləndi", balance = user.Balance });
        }
    }
}
