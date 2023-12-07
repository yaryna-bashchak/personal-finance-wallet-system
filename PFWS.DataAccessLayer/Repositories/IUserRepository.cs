using Microsoft.AspNetCore.Identity;
using PFWS.DataAccessLayer.Models;

namespace PFWS.DataAccessLayer.Repositories;

public interface IUserRepository
{
    Task<IdentityResult> Create(User user, string password);
    Task<User> FindByName(string username);
    Task<bool> CheckPassword(User user, string password);
}
