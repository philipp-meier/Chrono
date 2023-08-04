using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using MediatR;

namespace Chrono.Application.Features.Categories;

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
        var entity = new Category { Name = request.Name };
        _context.Categories.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
