using PFWS.BusinessLogicLayer.DTOs.Category;

namespace PFWS.BusinessLogicLayer.Services;

public interface ICategoryService
{
    public Task<List<GetCategoryDto>> GetCategories();
    public Task<GetCategoryDto> GetCategoryById(int id);
    public Task AddCategory(AddCategoryDto newCategory);
    public Task UpdateCategory(int id, UpdateCategoryDto updatedCategory);
    public Task DeleteCategory(int id);
}
