using Chrono.Common.Api;
using Chrono.Common.Interfaces;
using Chrono.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.Features.Notes;

public record CreateNote(string Title, string Text) : IRequest<int>;

public class CreateNoteValidator : AbstractValidator<CreateNote>
{
    public CreateNoteValidator(IApplicationDbContext dbContext)
    {
        RuleFor(v => v.Title)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Text)
            .NotEmpty();
    }
}

public class CreateNoteHandler : IRequestHandler<CreateNote, int>
{
    private readonly IApplicationDbContext _context;

    public CreateNoteHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateNote request, CancellationToken cancellationToken)
    {
        var entity = new Note
        {
            Title = request.Title, Text = request.Text
        };
        _context.Notes.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

[Authorize] [Route("api/notes")] [Tags("Notes")]
public class CreateNoteController : ApiControllerBase
{
    [HttpPost]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<int>> Create(CreateNote command)
    {
        return await Mediator.Send(command);
    }
}
