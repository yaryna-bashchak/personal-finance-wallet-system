using Microsoft.AspNetCore.Identity;
using PFWS.BusinessLogicLayer.DTOs.Auth;
using PFWS.DataAccessLayer.Models;

namespace PFWS.BusinessLogicLayer.Services;

public interface IAuthService
{
    public Task<LoginedUserDto> Login(LoginDto loginDto);
    public Task<IdentityResult> Register(RegisterDto registerDto);
    public Task<LoginedUserDto> GetCurrentUser(string username);
    public string GenerateJwtToken(User user);
}
