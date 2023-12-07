using PFWS.BusinessLogicLayer.DTOs.User;

namespace PFWS.BusinessLogicLayer.Services;

public interface IUserService
{
    public Task<List<GetUserDto>> GetUsers();
    public Task<GetUserDto> GetUserById(int id);
    public Task UpdateUser(UpdateUserDto updatedUser, string username);
    public Task DeleteUser(string username);
}
