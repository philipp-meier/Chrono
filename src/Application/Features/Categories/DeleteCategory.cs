using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.Features.Categories;

public record DeleteCategory(int Id) : IRequest;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategory>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCategoryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteCategory request, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Category \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        // Ensures only the TaskCategories are getting deleted and not the actual tasks too.
        _context.TaskCategories.RemoveRange(entity.Tasks);
        _context.Categories.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
