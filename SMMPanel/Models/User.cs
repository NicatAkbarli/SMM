using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMMPanel.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }

        // Refresh token üçün
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public decimal Balance { get; set; } = 0; // ₼ balans

        public string Role { get; set; } = "User"; // default rol
    }
}
