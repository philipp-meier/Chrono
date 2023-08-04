using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.Features.TaskLists;

public record DeleteTaskList(int Id) : IRequest;

public class DeleteTaskListHandler : IRequestHandler<DeleteTaskList>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskList request, CancellationToken cancellationToken)
    {
        var entity = await _context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task list \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        _context.Tasks.RemoveRange(entity.Tasks);
        _context.TaskLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
