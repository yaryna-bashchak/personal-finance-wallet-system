using PFWS.BusinessLogicLayer.DTOs.Category;

namespace PFWS.BusinessLogicLayer.Services;

public interface ICategoryService
{
    public Task<List<GetCategoryDto>> GetCategories(string username);
    public Task<GetCategoryDto> GetCategoryById(int id, string username);
    public Task AddCategory(AddCategoryDto newCategory, string username);
    public Task UpdateCategory(int id, UpdateCategoryDto updatedCategory, string username);
    public Task DeleteCategory(int id, string username);
}
