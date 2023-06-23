using MediatR;
using Chrono.Domain.Entities;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<int>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category { Name = request.Name };
        _context.Categories.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
