namespace PFWS.BusinessLogicLayer.DTOs.Category;

public class GetCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
