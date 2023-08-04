using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using MediatR;

namespace Chrono.Application.Features.Categories;

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
