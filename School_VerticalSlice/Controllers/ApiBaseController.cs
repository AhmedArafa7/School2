using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using School_VerticalSlice.Shared;

namespace School_VerticalSlice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : Controller
    {
        private ISender? _sender;
        protected ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

        // 1. إرسال Request بيرجع Result<T>
        protected async Task<ActionResult> SendAsync<TResponse>(
            IRequest<Result<TResponse>> request,
            CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        // 2. إرسال Request بيرجع Result بدون قيمة (204 NoContent عند النجاح)
        protected async Task<ActionResult> SendAsync(
            IRequest<Result> request,
            CancellationToken cancellationToken = default)
        {
            var result = await Sender.Send(request, cancellationToken);
            return HandleResult(result);
        }

        // 3. دمج Id من الـ Route مع Request من الـ Body (يرجع Result<T>)
        protected async Task<ActionResult> SendWithIdAsync<TId, TRequest, TCommand, TResponse>(
            TId id,
            TRequest bodyRequest,
            Func<TId, TRequest, TCommand> factory,
            CancellationToken cancellationToken = default)
            where TCommand : IRequest<Result<TResponse>>
        {
            var command = factory(id, bodyRequest);
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        // 4. دمج Id من الـ Route مع Request من الـ Body (يرجع Result بدون قيمة)
        protected async Task<ActionResult> SendWithIdAsync<TId, TRequest, TCommand>(
            TId id,
            TRequest bodyRequest,
            Func<TId, TRequest, TCommand> factory,
            CancellationToken cancellationToken = default)
            where TCommand : IRequest<Result>
        {
            var command = factory(id, bodyRequest);
            var result = await Sender.Send(command, cancellationToken);
            return HandleResult(result);
        }

        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return NoContent();
            else
                return HandleProblem(result.Errors);
        }

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (!result.IsSuccess)
                return HandleProblem(result.Errors);

            if (result.Value is string message)
            {
                return Ok(new { message });
            }

            return Ok(result.Value);
        }

        private ActionResult HandleProblem(IReadOnlyList<Error> errors)
        {
            if (errors.Count == 0)
                return Problem(statusCode: StatusCodes.Status500InternalServerError,
                    title: "Internal Server Error",
                    detail: "Unexpected Error Occured !");

            if (errors.All(E => E.Type == ErrorType.Validation))
                return HandleValidationProblem(errors);

            return HandleSingleErrorProblem(errors[0]);
        }

        private ActionResult HandleSingleErrorProblem(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.Type));
        }

        private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
                modelState.AddModelError(error.Code, error.Description);
            return ValidationProblem(modelState);
        }
    }
}
