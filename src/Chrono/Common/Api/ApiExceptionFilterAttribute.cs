using Chrono.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chrono.Common.Api;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        HandleException(context);

        base.OnException(context);
    }

    private static void HandleException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ValidationException validationException:
                HandleValidationException(context, validationException);
                break;
            case NotFoundException notFountException:
                HandleNotFoundException(context, notFountException);
                break;
            case InvalidOperationException invalidOperationException:
                HandleInvalidOperationException(context, invalidOperationException);
                break;
            case ForbiddenAccessException:
                HandleForbiddenAccessException(context);
                break;
        }
    }

    private static void HandleValidationException(ExceptionContext context, ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
            .ToDictionary(group => group.Key, group => group.ToArray());

        var details = new ValidationProblemDetails(errors)
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private static void HandleNotFoundException(ExceptionContext context, Exception exception)
    {
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4", Title = "The specified resource was not found.", Detail = exception.Message
        };

        context.Result = new NotFoundObjectResult(details);
        context.ExceptionHandled = true;
    }

    private static void HandleInvalidOperationException(ExceptionContext context, Exception exception)
    {
        var details = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1", Detail = exception.Message
        };

        context.Result = new BadRequestObjectResult(details);
        context.ExceptionHandled = true;
    }

    private static void HandleForbiddenAccessException(ExceptionContext context)
    {
        var details = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden, Title = "Forbidden", Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        context.Result = new ObjectResult(details)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
        context.ExceptionHandled = true;
    }
}
