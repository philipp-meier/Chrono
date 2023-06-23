using MediatR;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(int Id) : IRequest;

public class UpdateTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks
            .Include(x => x.List)
                .ThenInclude(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        
        if (!entity.IsPermitted(_currentUserService.UserId))
            throw new ForbiddenAccessException();

        if (entity.Done)
            throw new InvalidOperationException("Task is already done.");

        var taskList = entity.List;
        taskList.Tasks.Remove(entity);

        _context.Tasks.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
