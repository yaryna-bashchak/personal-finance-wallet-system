using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.User;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

public class UsersController : BaseApiController
{
private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<GetUserDto>>> GetUsers()
    {
        try
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateUserDto updatedUser)
    {
        try
        {
            var username = User.Identity.Name;
            await _userService.UpdateUser(updatedUser, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> DeleteUser()
    {
        try
        {
            var username = User.Identity.Name;
            await _userService.DeleteUser(username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
