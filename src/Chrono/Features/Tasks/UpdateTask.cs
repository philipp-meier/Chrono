using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using Chrono.Entities.Common;
using Chrono.Features.Categories;
using Chrono.Features.TaskLists;
using Chrono.Features.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.Tasks;

public record UpdateTask : IRequest
{
    public int Id { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public bool Done { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class UpdateTaskValidator : AbstractValidator<UpdateTask>
{
    public UpdateTaskValidator(IApplicationDbContext dbContext)
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty();

        RuleFor(v => v.Categories)
            .NotNull();

        RuleForEach(v => v.Categories)
            .ChildRules(child => child.RuleFor(x => x.Name).NotEmpty());

        RuleFor(v => v.BusinessValue)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.Id)?.RequireBusinessValue ?? false);

        RuleFor(v => v.Description)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.Id)?.RequireDescription ?? false);
    }

    private static TaskListOptions GetTaskListOptions(IApplicationDbContext dbContext, int taskId)
    {
        var task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskId);
        return task?.List?.Options;
    }
}

public class UpdateTaskHandler : IRequestHandler<UpdateTask>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateTask request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(x => x.List)
            .ThenInclude(x => x.Tasks)
            .ThenInclude(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        }

        if (!task.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        if (task.Done)
        {
            throw new InvalidOperationException("Task is already done.");
        }

        if (task.Done != request.Done && request.Done)
        {
            task.Done = true;
            task.Position = (task.List.Tasks.Where(x => x.Done).MaxBy(x => x.Position)?.Position ?? 0) + 1;
        }
        else
        {
            task.Name = request.Name;
            task.BusinessValue = request.BusinessValue;
            task.Description = request.Description;

            var newCategoryNames = request.Categories.Select(x => x.Name).ToArray();
            task.SetCategories(
                _context.Categories
                    .Where(x => newCategoryNames.Contains(x.Name))
                    .AsEnumerable()
                    .Where(x => x.IsPermitted(_currentUserService.UserId))
                    .ToArray()
            );

            if (task.Position != request.Position)
            {
                var taskList = task.List;
                taskList.InsertAt(request.Position, task);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize] [Route("api/tasks")]
public class UpdateTaskController : ApiControllerBase
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateTask command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }
}
