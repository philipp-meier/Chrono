using Chrono.Entities;
using Chrono.Shared.Api;
using Chrono.Shared.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.TaskLists;

public record CreateTaskList(string Title) : IRequest<int>;

public class CreateTaskListValidator : AbstractValidator<CreateTaskList>
{
    public CreateTaskListValidator()
    {
        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();
    }
}

public class CreateTaskListHandler(IApplicationDbContext context) : IRequestHandler<CreateTaskList, int>
{
    public async Task<int> Handle(CreateTaskList request, CancellationToken cancellationToken)
    {
        var entity = new TaskList { Title = request.Title };
        context.TaskLists.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

[Authorize]
[Route("api/tasklists")]
[Tags("Tasklists")]
public class CreateTaskListController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> Create(CreateTaskList command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtRoute("GetTaskList", new { id = result }, JSendResponseBuilder.Success(result));
    }
}
