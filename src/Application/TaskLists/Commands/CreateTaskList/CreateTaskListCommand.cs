using MediatR;
using Chrono.Domain.Entities;
using Chrono.Application.Common.Interfaces;

namespace Chrono.Application.TaskLists.Commands.CreateTaskList;

public record CreateTaskListCommand(string Title) : IRequest<int>;

public class CreateTaskListCommandHandler : IRequestHandler<CreateTaskListCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateTaskListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TaskList { Title = request.Title };
        _context.TaskLists.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
