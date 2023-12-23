using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Tasks;

public record GetTask(int Id) : IRequest<TaskDto>;

public class GetTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<GetTask, TaskDto>
{
    public async Task<TaskDto> Handle(GetTask request, CancellationToken cancellationToken)
    {
        var task = await context.Tasks
            .Include(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new NotFoundException($"Task \"{request.Id}\" not found.");
        }

        if (!task.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskDto.FromEntity(task);
    }
}

[Authorize]
[Route("api/tasks")]
[Tags("Tasks")]
public class GetTaskController : ApiControllerBase
{
    [HttpGet("{id:int}", Name = "GetTask")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await Mediator.Send(new GetTask(id));
        return Ok(JSendResponseBuilder.Success(result));
    }
}
