using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Security;
using Chrono.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Application.Tasks.Commands.UpdateTaskList;

public record UpdateTaskListCommand : IRequest
{
    public int TaskListId { get; init; }
    public string Title { get; init; }
    public bool? RequireBusinessValue { get; init; }
    public bool? RequireDescription { get; init; }
}

public class UpdateTaskListCommandHandler : IRequestHandler<UpdateTaskListCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskListCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateTaskListCommand request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .SingleOrDefaultAsync(x => x.Id == request.TaskListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.TaskListId}\" not found.");
        }

        if (!taskList.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        if (taskList.Title != request.Title)
        {
            taskList.Title = request.Title;
        }

        var options = taskList.Options;
        if (options == null)
        {
            options = new TaskListOptions { TaskList = taskList, TaskListId = taskList.Id };
            _context.TaskListOptions.Add(options);
        }

        options.RequireBusinessValue = request.RequireBusinessValue.GetValueOrDefault();
        options.RequireDescription = request.RequireDescription.GetValueOrDefault();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
