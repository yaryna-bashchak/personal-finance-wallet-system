namespace PFWS.DataAccessLayer.Models;

public interface IEntityBaseOrIdentityUser
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
