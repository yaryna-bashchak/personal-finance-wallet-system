using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFWS.API.Controllers.Controllers;
using PFWS.BusinessLogicLayer.DTOs.Category;
using PFWS.BusinessLogicLayer.Services;

namespace PFWS.API.Controllers;

[Authorize]
public class CategoriesController : BaseApiController
{
    private readonly ICategoryService _categoryService;
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCategoryDto>> GetCategoryById(int id)
    {
        try
        {
            var username = User.Identity.Name;
            var category = await _categoryService.GetCategoryById(id, username);
            return Ok(category);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ActionResult<GetCategoryDto>> GetCategories()
    {
        try
        {
            var username = User.Identity.Name;
            var categories = await _categoryService.GetCategories(username);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddCategory(AddCategoryDto newCategory)
    {
        try
        {
            var username = User.Identity.Name;
            await _categoryService.AddCategory(newCategory, username);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto updatedCategory)
    {
        try
        {
            var username = User.Identity.Name;
            await _categoryService.UpdateCategory(id, updatedCategory, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var username = User.Identity.Name;
            await _categoryService.DeleteCategory(id, username);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
