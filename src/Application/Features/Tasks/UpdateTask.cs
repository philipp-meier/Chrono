using Chrono.Application.Common.Dtos;
using Chrono.Application.Common.Exceptions;
using Chrono.Application.Common.Extensions;
using Chrono.Application.Common.Interfaces;
using Chrono.Application.Common.Services;
using Chrono.Application.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Chrono.Application.Features.Tasks;

public record UpdateTask : IRequest
{
    public int Id { get; init; }
    public int Position { get; init; }
    public string Name { get; init; }
    public string BusinessValue { get; init; }
    public string Description { get; init; }
    public bool Done { get; init; }
    public CategoryDto[] Categories { get; init; }
}

public class UpdateTaskHandler : IRequestHandler<UpdateTask>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public UpdateTaskHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(UpdateTask request, CancellationToken cancellationToken)
    {
        var entity = await _context.Tasks
            .Include(x => x.List)
            .ThenInclude(x => x.Tasks)
            .ThenInclude(x => x.Categories)
            .ThenInclude(x => x.Category)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Task item \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(_currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        if (entity.Done)
        {
            throw new InvalidOperationException("Task is already done.");
        }

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
            {
                TaskListService.InsertAt(request.Position, entity);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

public class UpdateTaskValidator : AbstractValidator<UpdateTask>
{
    public UpdateTaskValidator(IApplicationDbContext dbContext)
    {
        RuleFor(v => v.Id)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty();

        RuleFor(v => v.Categories)
            .NotNull();

        RuleForEach(v => v.Categories)
            .ChildRules(child => child.RuleFor(x => x.Name).NotEmpty());

        RuleFor(v => v.BusinessValue)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.Id)?.RequireBusinessValue ?? false);

        RuleFor(v => v.Description)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.Id)?.RequireDescription ?? false);
    }

    private TaskListOptions GetTaskListOptions(IApplicationDbContext dbContext, int taskId)
    {
        var task = dbContext.Tasks.FirstOrDefault(x => x.Id == taskId);
        return task?.List?.Options;
    }
}
