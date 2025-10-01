using SMMPanel.Dtos;
using SMMPanel.Models;

namespace SMMPanel.Services
{
   public interface IAuthService
{
    Task<string> Register(UserRegisterDto request);
    Task<(string token, string refreshToken)> Login(UserLoginDto request);
    string CreateToken(User user);
}

}
