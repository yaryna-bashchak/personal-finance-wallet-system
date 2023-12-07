namespace PFWS.BusinessLogicLayer.DTOs.User;

public class GetUserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
