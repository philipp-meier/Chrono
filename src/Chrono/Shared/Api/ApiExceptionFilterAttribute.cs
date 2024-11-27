using Chrono.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chrono.Shared.Api;

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
            default:
                HandleInternalServerErrorException(context);
                break;
        }
    }

    private static void HandleValidationException(ExceptionContext context, ValidationException exception)
    {
        var result = new
        {
            ValidationErrors = exception.Errors
                .GroupBy(x => x.PropertyName, x => x.ErrorMessage)
                .ToDictionary(group => group.Key, group => group.ToArray())
        };

        context.Result = new BadRequestObjectResult(JSendResponseBuilder.Fail(result));
        context.ExceptionHandled = true;
    }

    private static void HandleNotFoundException(ExceptionContext context, NotFoundException notFoundException)
    {
        context.Result = new NotFoundObjectResult(JSendResponseBuilder.Error<string>(notFoundException.Message));
        context.ExceptionHandled = true;
    }

    private static void HandleInvalidOperationException(ExceptionContext context,
        InvalidOperationException invalidOperationException)
    {
        context.Result =
            new BadRequestObjectResult(JSendResponseBuilder.Error<string>(invalidOperationException.Message));
        context.ExceptionHandled = true;
    }

    private static void HandleForbiddenAccessException(ExceptionContext context)
    {
        context.Result = new ObjectResult(JSendResponseBuilder.Error<string>("Forbidden"))
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
        context.ExceptionHandled = true;
    }

    private static void HandleInternalServerErrorException(ExceptionContext context)
    {
        context.Result = new ObjectResult(JSendResponseBuilder.Error<string>("Internal server error"))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
        context.ExceptionHandled = true;
    }
}
