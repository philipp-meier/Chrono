using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Extensions;
using Chrono.Common.Interfaces;
using Chrono.Common.Services;
using Chrono.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.Tasks;

public record CreateTask : IRequest<int>
{
    public int ListId { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class CreateTaskValidator : AbstractValidator<CreateTask>
{
    public CreateTaskValidator(IApplicationDbContext dbContext)
    {
        RuleFor(v => v.ListId)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty();

        RuleFor(v => v.Categories)
            .NotNull();

        RuleFor(v => v.BusinessValue)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.ListId)?.RequireBusinessValue ?? true);

        RuleFor(v => v.Description)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.ListId)?.RequireDescription ?? true);

        RuleForEach(v => v.Categories)
            .ChildRules(child => child.RuleFor(x => x.Name).NotEmpty());
    }

    private static TaskListOptions GetTaskListOptions(IApplicationDbContext dbContext, int taskListId)
    {
        return dbContext.TaskLists.FirstOrDefault(x => x.Id == taskListId)?.Options;
    }
}

public class CreateTaskHandler : IRequestHandler<CreateTask, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<int> Handle(CreateTask request, CancellationToken cancellationToken)
    {
        var taskList = await _context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        var task = new Task
        {
            Name = request.Name, Position = request.Position, BusinessValue = request.BusinessValue, Description = request.Description
        };
        var newCategoryNames = request.Categories.Select(x => x.Name).ToArray();
        task.SetCategories(
            _context.Categories
                .Where(x => newCategoryNames.Contains(x.Name))
                .AsEnumerable()
                .Where(x => x.IsPermitted(_currentUserService.UserId))
                .ToArray()
        );

        taskList.InsertAt(request.Position, task);

        _context.Tasks.Add(task);

        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}

[Authorize] [Route("api/tasks")] [Tags("Tasks")]
public class CreateTaskController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(CreateTask command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtRoute("GetTask", new
        {
            id = result
        }, JSendResponseBuilder.Success(result));
    }
}
