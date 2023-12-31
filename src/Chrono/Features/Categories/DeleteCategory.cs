﻿using Chrono.Shared.Api;
using Chrono.Shared.Exceptions;
using Chrono.Shared.Extensions;
using Chrono.Shared.Interfaces;
using Chrono.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chrono.Features.Categories;

public record DeleteCategory(int Id) : IRequest;

public class DeleteCategoryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
    : IRequestHandler<DeleteCategory>
{
    public async Task Handle(DeleteCategory request, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .Include(x => x.Tasks)
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException($"Category \"{request.Id}\" not found.");
        }

        if (!entity.IsPermitted(currentUserService.UserId))
        {
            throw new ForbiddenAccessException();
        }

        // Ensures only the TaskCategories are getting deleted and not the actual tasks too.
        context.TaskCategories.RemoveRange(entity.Tasks);
        context.Categories.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);
    }
}

[Authorize]
[Route("api/categories")]
[Tags("Categories")]
public class DeleteCategoryController : ApiControllerBase
{
    [HttpDelete("{id:int}")]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteCategory(id));
        return Ok(JSendResponseBuilder.Success<string>(null));
    }
}
