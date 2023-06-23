using MediatR;
using Chrono.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Security;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.Tasks.Commands.UpdateTask;

public record UpdateTaskCommand : IRequest
{
    public int Id { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public bool Done { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks
            .Include(x => x.List)
                .ThenInclude(x => x.Tasks)
                .ThenInclude(x => x.Categories)
                .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        
        if (!entity.IsPermitted(_currentUserService.UserId))
            throw new ForbiddenAccessException();

        if (entity.Done)
            throw new InvalidOperationException("Task is already done.");

        if (entity.Done != request.Done && request.Done)
        {
            entity.Done = true;
            entity.Position = (entity.List.Tasks.Where(x => x.Done).MaxBy(x => x.Position)?.Position ?? 0) + 1;
        }
        else
        {
            entity.Name = request.Name;
            entity.BusinessValue = request.BusinessValue;
            entity.Description = request.Description;

            var newCategoryNames = request.Categories.Select(x => x.Name).ToArray();
            TaskService.SetCategories(entity,
                _context.Categories
                .Where(x => newCategoryNames.Contains(x.Name))
                .AsEnumerable()
                .Where(x => x.IsPermitted(_currentUserService.UserId))
                .ToArray()
            );

            if (entity.Position != request.Position)
                TaskListService.InsertAt(request.Position, entity);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
