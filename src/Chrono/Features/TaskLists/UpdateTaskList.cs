using Chrono.Entities;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record UpdateTaskList
{
    public int TaskListId { get; init; }
    public string Title { get; init; }
    public bool? RequireBusinessValue { get; init; }
    public bool? RequireDescription { get; init; }
}

public class UpdateTaskListValidator : Validator<UpdateTaskList>
{
    public UpdateTaskListValidator()
    {
        RuleFor(v => v.TaskListId)
            .NotEmpty();

        RuleFor(v => v.Title)
            .NotEmpty();

        RuleFor(v => v.RequireBusinessValue)
            .NotNull();

        RuleFor(v => v.RequireDescription)
            .NotNull();
    }
}

[Authorize]
[HttpPut("api/tasklists/{id:int}")]
[Tags("Tasklists")]
public class UpdateTaskListEndpoint(IApplicationDbContext context, ICurrentUserService currentUserService)
    : Endpoint<UpdateTaskList>
{
    public override async Task HandleAsync(UpdateTaskList request, CancellationToken cancellationToken)
    {
        var taskList = await context.TaskLists
            .SingleOrDefaultAsync(x => x.Id == request.TaskListId, cancellationToken);

        if (taskList == null)
        {
            throw new NotFoundException($"Task list \"{request.TaskListId}\" not found.");
        }

        if (!taskList.IsPermitted(currentUserService.UserId))
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
            context.TaskListOptions.Add(options);
        }

        options.RequireBusinessValue = request.RequireBusinessValue.GetValueOrDefault();
        options.RequireDescription = request.RequireDescription.GetValueOrDefault();

        await context.SaveChangesAsync(cancellationToken);
        await SendOkAsync(cancellationToken);
    }
}
