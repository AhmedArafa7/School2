using FluentValidation;
using School_VerticalSlice.Shared;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace School_VerticalSlice.Infrastructure.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next(); 
////i need to understand that code
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errors = failures.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage)).ToList();

                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var failMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Fail", new[] { typeof(List<Error>) });
                    return (TResponse)failMethod!.Invoke(null, new object[] { errors })!;
                }
                else if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.Fail(errors);
                }
            }

            return await next(); // البيانات سليمة، اذهب للـ Handler        
        }
    }
}
