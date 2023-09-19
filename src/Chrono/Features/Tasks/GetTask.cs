using Chrono.Common.Api;
using Chrono.Common.Exceptions;
using Chrono.Common.Interfaces;
using Chrono.Features.Audit;
using Chrono.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Tasks;

[Authorize] [Route("api/tasks")]
public class GetTaskController : ApiControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDto>> Get(int id)
    {
        return await Mediator.Send(new GetTask(id));
    }
}

public record GetTask(int Id) : IRequest<TaskDto>;

public class GetTaskHandler : IRequestHandler<GetTask, TaskDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(GetTask request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
        {
            throw new NotFoundException($"Task \"{request.Id}\" not found.");
        }

        if (!task.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        return TaskDto.FromEntity(task);
    }
}
