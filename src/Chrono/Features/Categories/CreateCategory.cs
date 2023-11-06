using Chrono.Shared.Api;
using Chrono.Shared.Interfaces;
using Chrono.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Categories;

public record CreateCategory(string Name) : IRequest<int>;

public class CreateCategoryHandler : IRequestHandler<CreateCategory, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCategory request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name
        };
        _context.Categories.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

[Authorize] [Route("api/categories")] [Tags("Categories")]
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
