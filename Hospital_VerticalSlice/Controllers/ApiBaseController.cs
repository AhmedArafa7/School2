using Hospital_VerticalSlice.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hospital_VerticalSlice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : Controller
    {
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
            // ModelState
            var ModelState = new ModelStateDictionary();
            foreach (var error in errors)
                ModelState.AddModelError(error.Code, error.Description);
            return ValidationProblem(ModelState);
        }
    }
}
