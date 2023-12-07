namespace PFWS.DataAccessLayer.Models;

public class EntityBase : IEntityBaseOrIdentityUser
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public EntityBase()
    {
        var date = DateTime.Now;
        CreatedAt = date;
        UpdatedAt = date;
    }
}
