using MediatR;
using Chrono.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<int>
{
    public int ListId { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");

        var entity = new Domain.Entities.Task
        {
            Name = request.Name,
            Position = request.Position,
            BusinessValue = request.BusinessValue,
            Description = request.Description
        };
        var newCategoryNames = request.Categories.Select(x => x.Name).ToArray();
        TaskService.SetCategories(entity,
            _context.Categories
            .Where(x => newCategoryNames.Contains(x.Name))
            .AsEnumerable()
            .Where(x => x.IsPermitted(_currentUserService.UserId))
            .ToArray()
        );

        TaskListService.InsertAt(request.Position, entity, targetTaskList: taskList);

        _context.Tasks.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
