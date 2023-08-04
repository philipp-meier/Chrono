using Chrono.Application.Common.Interfaces;
using Chrono.Application.Entities;
using FluentValidation;
using MediatR;

namespace Chrono.Application.Features.TaskLists;

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
        var entity = new TaskList { Title = request.Title };
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
