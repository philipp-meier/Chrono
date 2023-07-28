using Chrono.Application.Tasks.Commands.UpdateTaskList;
using FluentValidation;

namespace Chrono.Application.TaskLists.Commands.UpdateTaskList;

public class UpdateTaskListCommandValidator : AbstractValidator<UpdateTaskListCommand>
{
    public UpdateTaskListCommandValidator()
    {
        RuleFor(v => v.TaskListId)
            .NotEmpty();

        RuleFor(v => v.Title)
            .NotEmpty();

        RuleFor(v => v.RequireBusinessValue)
            .NotNull();

        RuleFor(v => v.RequireDescription)
            .NotNull();
    }
}
