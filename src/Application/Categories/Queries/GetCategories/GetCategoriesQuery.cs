using MediatR;
using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<CategoryDto[]>;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, CategoryDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public Task<CategoryDto[]> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
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
