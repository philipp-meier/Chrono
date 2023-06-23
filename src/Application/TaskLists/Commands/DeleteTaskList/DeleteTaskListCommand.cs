using MediatR;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Security;

namespace Chrono.Application.TaskLists.Commands.DeleteTaskList;

public record DeleteTaskListCommand(int Id) : IRequest;

public class DeleteTaskListCommandHandler : IRequestHandler<DeleteTaskListCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteTaskListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskListCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"Task list \"{request.Id}\" not found.");
        
        if (!entity.IsPermitted(_currentUserService.UserId))
            throw new ForbiddenAccessException();

        _context.Tasks.RemoveRange(entity.Tasks);
        _context.TaskLists.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
