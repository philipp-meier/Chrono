using Chrono.Application.Common.Interfaces;
using Chrono.Domain.Entities;
using FluentValidation;

namespace Chrono.Application.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator(IApplicationDbContext dbContext)
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
