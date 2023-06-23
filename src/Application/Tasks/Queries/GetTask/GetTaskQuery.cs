using MediatR;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Exceptions;

namespace Chrono.Application.Tasks.Queries.GetTask;

public record GetTaskQuery(int Id) : IRequest<TaskDto>;

public class GetTaskQueryHandler : IRequestHandler<GetTaskQuery, TaskDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetTaskQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TaskDto> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks
            .Include(x => x.Categories)
                .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (task == null)
            throw new NotFoundException($"Task \"{request.Id}\" not found.");
        
        if (!task.IsPermitted(_currentUserService.UserId))
            throw new ForbiddenAccessException();

        return TaskDto.FromEntity(task);
    }
}
