using Microsoft.AspNetCore.Identity;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Repositories.Implementation;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityResult> Create(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<User> FindByName(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<bool> CheckPassword(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }
}
