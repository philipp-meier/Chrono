using Chrono.Application.Common.Interfaces;
using Chrono.Domain.Entities;
using FluentValidation;

namespace Chrono.Application.Tasks.Commands.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator(IApplicationDbContext dbContext)
    {
        RuleFor(v => v.ListId)
            .NotEmpty();

        RuleFor(v => v.Position)
            .NotEmpty();

        RuleFor(v => v.Name)
            .NotEmpty();

        RuleFor(v => v.Categories)
            .NotNull();

        RuleFor(v => v.BusinessValue)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.ListId)?.RequireBusinessValue ?? true);

        RuleFor(v => v.Description)
            .NotEmpty()
            .When(x => GetTaskListOptions(dbContext, x.ListId)?.RequireDescription ?? true);

        RuleForEach(v => v.Categories)
            .ChildRules(child => child.RuleFor(x => x.Name).NotEmpty());
    }

    private TaskListOptions GetTaskListOptions(IApplicationDbContext dbContext, int taskListId)
    {
        return dbContext.TaskLists.FirstOrDefault(x => x.Id == taskListId)?.Options;
    }
}
