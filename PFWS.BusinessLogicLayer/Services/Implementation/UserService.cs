using PFWS.BusinessLogicLayer.DTOs.User;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRepositoryBase<User> _repositoryBase;

    public UserService(IRepositoryBase<User> repositoryBase, IUserRepository userRepository)
    {
        _repositoryBase = repositoryBase;
        _userRepository = userRepository;
    }

    public async Task<GetUserDto> GetUserById(int id)
    {
        var user = await _repositoryBase.GetItem(id);
        if (user == null)
            throw new Exception("User not found");

        return MapToUserDto(user);
    }

    public async Task<List<GetUserDto>> GetUsers()
    {
        var allUsers = await _repositoryBase.GetAllItems();
        return allUsers.Select(MapToUserDto).ToList();
    }

    public async Task UpdateUser(UpdateUserDto updatedUser, string username)
    {
        var user = await GetUserByUsername(username);

        user.UserName = updatedUser.Username;

        await _repositoryBase.UpdateItem(user.Id, user);
    }

    public async Task DeleteUser(string username)
    {
        var user = await GetUserByUsername(username);

        await _repositoryBase.DeleteItem(user);
    }

    private async Task<User> GetUserByUsername(string username)
    {
        var user = await _userRepository.FindByName(username);
        if (user == null)
            throw new Exception("User not found");
        if (user.UserName != username)
            throw new Exception("Unauthorized access to the user");

        return user;
    }

    private GetUserDto MapToUserDto(User user)
    {
        return new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
    }
}
