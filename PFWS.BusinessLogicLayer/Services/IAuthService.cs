using PFWS.BusinessLogicLayer.DTOs.Auth;

namespace PFWS.BusinessLogicLayer.Services;

public interface IAuthService
{
    public Task<GetTokenDto> Login(LoginDto loginDto);
    public Task Register(RegisterDto registerDto);
    public Task<string> GenerateToken(LoginDto loginDto);
}
