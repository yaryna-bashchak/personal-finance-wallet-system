using PFWS.BusinessLogicLayer.DTOs.Category;
using PFWS.DataAccessLayer.Models;
using PFWS.DataAccessLayer.Repositories;

namespace PFWS.BusinessLogicLayer.Services.Implementation;

public class CategoryService : ICategoryService
{
    private readonly IRepositoryBase<Category> _repositoryBase;
    public readonly IUserRepository _userRepository;
    public CategoryService(IRepositoryBase<Category> repositoryBase, IUserRepository userRepository)
    {
        _repositoryBase = repositoryBase;
        _userRepository = userRepository;
    }

    public async Task AddCategory(AddCategoryDto newCategory, string username)
    {
        await GetUserByUsername(username);

        if (!(newCategory.Type == "income" || newCategory.Type == "expense"))
            throw new Exception("Type should be only either 'income' or 'expense'");

        var category = new Category
        {
            Name = newCategory.Name,
            Type = newCategory.Type,
        };

        await _repositoryBase.AddItem(category);
    }

    public async Task DeleteCategory(int id, string username)
    {
        await GetUserByUsername(username);
        var category = await GetCategory(id);

        await _repositoryBase.DeleteItem(category);
    }

    public async Task<List<GetCategoryDto>> GetCategories(string username)
    {
        var user = await GetUserByUsername(username);
        var categories = await _repositoryBase.GetAllItems();

        var categoriesDto = categories.Select(MapToCategoryDto).ToList();

        return categoriesDto;
    }

    public async Task<GetCategoryDto> GetCategoryById(int id, string username)
    {
        await GetUserByUsername(username);
        var category = await GetCategory(id);

        var categoryDto = MapToCategoryDto(category);

        return categoryDto;
    }

    public async Task UpdateCategory(int id, UpdateCategoryDto updatedCategory, string username)
    {
        await GetUserByUsername(username);
        var category = await GetCategory(id);

        category.Name = updatedCategory.Name;

        await _repositoryBase.UpdateItem(id, category);
    }

    private async Task<User> GetUserByUsername(string username)
    {
        var user = await _userRepository.FindByName(username);
        if (user == null)
            throw new Exception("User not found");
        return user;
    }
    private async Task<Category> GetCategory(int id)
    {
        var category = await _repositoryBase.GetItem(id);
        if (category == null)
            throw new Exception("Category not found");

        return category;
    }

    private GetCategoryDto MapToCategoryDto(Category category)
    {
        return new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Type = category.Type,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }
}
