namespace PFWS.BusinessLogicLayer.DTOs.Auth;

public class LoginedUserDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
}
