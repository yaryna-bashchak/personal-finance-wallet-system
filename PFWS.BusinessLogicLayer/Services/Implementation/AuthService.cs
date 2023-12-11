using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PFWS.BusinessLogicLayer.DTOs.Auth;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository userRepository, IConfiguration config)
    {
        _userRepository = userRepository;
        _config = config;
    }

    public async Task<IdentityResult> Register(RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Username
        };

        var result = await _userRepository.CreateAsync(user, registerDto.Password);
        return result;
    }

    public async Task<LoginedUserDto> Login(LoginDto loginDto)
    {
        var user = await _userRepository.FindByNameAsync(loginDto.Username);

        if (user == null || !await _userRepository.CheckPasswordAsync(user, loginDto.Password))
            throw new Exception("Login failed");

        return new LoginedUserDto
        {
            UserId = user.Id,
            Username = user.UserName,
            Token = GenerateJwtToken(user),
        };
    }

    public async Task<LoginedUserDto> GetCurrentUser(string username)
    {
        var user = await _userRepository.FindByNameAsync(username);
        if (user == null)
            throw new Exception("Current user is invalid");

        return new LoginedUserDto
        {
            UserId = user.Id,
            Username = user.UserName,
            Token = GenerateJwtToken(user)
        };
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenOptions = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}
