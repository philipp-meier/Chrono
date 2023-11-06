using Chrono.Shared.Api;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Categories;

public record GetCategories : IRequest<CategoryDto[]>;

public class GetCategoriesHandler : IRequestHandler<GetCategories, CategoryDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<CategoryDto[]> Handle(GetCategories request, CancellationToken cancellationToken)
    {
        var result = _context.Categories
            .OrderBy(x => x.Name)
            .AsEnumerable()
            .Where(x => x.IsPermitted(_currentUserService.UserId))
            .Select(CategoryDto.FromEntity)
            .ToArray();

        return Task.FromResult(result);
    }
}

[Authorize] [Route("api/categories")] [Tags("Categories")]
public class GetCategoriesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetCategories());
        return Ok(JSendResponseBuilder.Success(result));
    }
}
