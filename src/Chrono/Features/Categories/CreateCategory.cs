using Chrono.Common.Api;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Categories;

[Authorize] [Route("api/categories")]
public class CreateCategoryController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateCategory command)
    {
        return await Mediator.Send(command);
    }
}

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
