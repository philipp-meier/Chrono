using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Application.Features.Tasks;

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
