using Microsoft.AspNetCore.Identity;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Repositories;

public interface IUserRepository
{
    Task<IdentityResult> CreateAsync(User user, string password);
    Task<User> FindByNameAsync(string username);
    Task<bool> CheckPasswordAsync(User user, string password);
}
