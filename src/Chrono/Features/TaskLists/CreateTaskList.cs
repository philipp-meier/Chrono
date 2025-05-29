using Chrono.Entities;
using Chrono.Shared.Interfaces;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Features.TaskLists;

public record CreateTaskList(string Title);

public class CreateTaskListValidator : Validator<CreateTaskList>
{
    public CreateTaskListValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();
    }
}

[Authorize]
[HttpPost("api/tasklists")]
[Tags("Tasklists")]
public class CreateTaskListEndpoint(IApplicationDbContext context) : Endpoint<CreateTaskList, int>
{
    public override async Task HandleAsync(CreateTaskList request, CancellationToken cancellationToken)
    {
        var entity = new TaskList { Title = request.Title };
        context.TaskLists.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        await SendCreatedAtAsync<GetTaskListEndpoint>(new { id = entity.Id }, entity.Id,
            cancellation: cancellationToken);
    }
}
