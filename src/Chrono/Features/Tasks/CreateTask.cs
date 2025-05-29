using Chrono.Entities;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Task = Chrono.Entities.Task;

namespace Chrono.Features.Tasks;

public record CreateTask
{
    public int ListId { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class CreateTaskValidator : Validator<CreateTask>
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

[Authorize]
[HttpPost("api/tasks")]
[Tags("Tasks")]
public class CreateTaskEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<CreateTask, int>
{
    public override async System.Threading.Tasks.Task HandleAsync(CreateTask request,
        CancellationToken cancellationToken)
    {
        var taskList = await context.TaskLists
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.ListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.ListId}\" not found.");
        }

        if (!taskList.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        var task = new Task
        {
            Name = request.Name,
            Position = request.Position,
            BusinessValue = request.BusinessValue,
            Description = request.Description
        };
        var newCategoryNames = request.Categories.Select(x => x.Name).ToArray();
        task.SetCategories(
            context.Categories
                .Where(x => newCategoryNames.Contains(x.Name))
                .AsEnumerable()
                .Where(x => x.IsPermitted(currentUserService.UserId))
                .ToArray()
        );

        taskList.InsertAt(request.Position, task);

        context.Tasks.Add(task);

        await context.SaveChangesAsync(cancellationToken);

        await SendCreatedAtAsync<GetTaskEndpoint>(new { id = task.Id }, task.Id, cancellation: cancellationToken);
    }
}
