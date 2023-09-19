using Chrono.Common.Api;
using Chrono.Common.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.TaskLists;

[Authorize] [Route("api/tasklists")]
public class CreateTaskListController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateTaskList command)
    {
        return await Mediator.Send(command);
    }
}

public record CreateTaskList(string Title) : IRequest<int>;

public class CreateTaskListHandler : IRequestHandler<CreateTaskList, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskListHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTaskList request, CancellationToken cancellationToken)
    {
        var entity = new TaskList
        {
            Title = request.Title
        };
        _context.TaskLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

public class CreateTaskListValidator : AbstractValidator<CreateTaskList>
{
    public CreateTaskListValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty();
    }
}
