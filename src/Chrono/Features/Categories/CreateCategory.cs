using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Categories;

public record CreateCategory(string Name) : IRequest<int>;

public class CreateCategoryHandler(IApplicationDbContext context) : IRequestHandler<CreateCategory, int>
{
    public async Task<int> Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        var entity = new Category { Name = request.Name };
        context.Categories.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

[Authorize]
[Route("api/categories")]
[Tags("Categories")]
public class CreateCategoryController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Create(CreateCategory command)
    {
        var result = await Mediator.Send(command);
        return CreatedAtRoute(null, JSendResponseBuilder.Success(result));
    }
}
