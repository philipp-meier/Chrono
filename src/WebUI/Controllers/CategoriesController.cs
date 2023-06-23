using Microsoft.AspNetCore.Mvc;
using Chrono.Application.Common.Dtos;
using Microsoft.AspNetCore.Authorization;
using Chrono.Application.Categories.Queries.GetCategories;
using Chrono.Application.Categories.Commands.CreateCategory;
using Chrono.Application.Categories.Commands.DeleteCategory;

namespace Chrono.WebUI.Controllers;

[Authorize]
public class CategoriesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CategoryDto[]>> Get()
        => await Mediator.Send(new GetCategoriesQuery());
    
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateCategoryCommand command)
        => await Mediator.Send(command);
    
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteCategoryCommand(id));
        return NoContent();
    }
}
