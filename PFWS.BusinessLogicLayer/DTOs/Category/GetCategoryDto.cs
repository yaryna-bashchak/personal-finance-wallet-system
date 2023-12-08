namespace PFWS.BusinessLogicLayer.DTOs.Category;

public class GetCategoryDto : GetCategoryDtoShort
{
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
