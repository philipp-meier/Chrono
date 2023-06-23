using MediatR;
using Chrono.WebUI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Chrono.WebUI.Controllers;

[ApiController, ApiExceptionFilter]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
