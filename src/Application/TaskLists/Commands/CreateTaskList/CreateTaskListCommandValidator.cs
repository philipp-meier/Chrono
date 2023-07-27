using FluentValidation;

namespace Chrono.Application.TaskLists.Commands.CreateTaskList;

public class CreateTaskListCommandValidator : AbstractValidator<CreateTaskListCommand>
{
    public CreateTaskListCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty();
    }
}
