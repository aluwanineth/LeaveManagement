using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using LeaveManagement.Application.Wrappers;
using LeaveManagement.Application.Exceptions;


namespace LeaveManagement.API.Attribute
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilterAttribute()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                return;
            }

            HandleUnknownException(context);
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = (ValidationException)context.Exception;

            // Convert our custom errors to ModelStateDictionary for ValidationProblemDetails
            var modelStateDictionary = new ModelStateDictionary();
            foreach (var error in exception.Errors)
            {
                foreach (var errorMessage in error.Value)
                {
                    modelStateDictionary.AddModelError(error.Key, errorMessage);
                }
            }

            var details = new ValidationProblemDetails(modelStateDictionary)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Instance = context.HttpContext.Request.Path
            };

            // Also create our custom response format
            var response = new Response<object>
            {
                Succeeded = false,
                Message = "Validation failed",
                Errors = exception.Errors.SelectMany(x => x.Value).ToList()
            };

            // You can choose to return either ValidationProblemDetails or custom Response
            // Option 1: Standard ValidationProblemDetails (RFC 7807 compliant)
            context.Result = new BadRequestObjectResult(details);

            // Option 2: Custom Response format (uncomment if preferred)
            // context.Result = new BadRequestObjectResult(response);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = (NotFoundException)context.Exception;

            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Status = StatusCodes.Status404NotFound,
                Detail = exception.Message,
                Instance = context.HttpContext.Request.Path
            };

            var response = new Response<object>(exception.Message);

            context.Result = new NotFoundObjectResult(response);
            context.ExceptionHandled = true;
        }

        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            };

            var response = new Response<object>("Unauthorized access");

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
            context.ExceptionHandled = true;
        }

        private void HandleForbiddenAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
            };

            var response = new Response<object>("Forbidden access");

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Validation failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Instance = context.HttpContext.Request.Path
            };

            var response = new Response<object>
            {
                Succeeded = false,
                Message = "Validation failed",
                Errors = context.ModelState
                    .SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? Enumerable.Empty<string>())
                    .ToList()
            };

            context.Result = new BadRequestObjectResult(response);
            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            };

            var response = new Response<object>("An error occurred while processing your request");

            context.Result = new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
