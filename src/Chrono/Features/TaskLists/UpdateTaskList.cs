using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record UpdateTaskList : IRequest
{
    public int TaskListId { get; init; }
    public string Title { get; init; }
    public bool? RequireBusinessValue { get; init; }
    public bool? RequireDescription { get; init; }
}

public class UpdateTaskListValidator : AbstractValidator<UpdateTaskList>
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

public class UpdateTaskListHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateTaskList>
{
    public async Task Handle(UpdateTaskList request, CancellationToken cancellationToken)
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
    }
}

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class UpdateTaskListController : ApiControllerBase
{
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Update(int id, UpdateTaskList command)
    {
        if (id != command.TaskListId)
        {
            return BadRequest(JSendResponseBuilder.Fail<string>(null, "ID mismatch"));
        }

        await Mediator.Send(command);

        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
