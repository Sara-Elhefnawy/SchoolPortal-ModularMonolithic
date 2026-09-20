using FluentValidation;
using MediatR;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.SharedKernel.Behaviors;

// Sits in front of every handler whose TRequest has a registered validator.
// If there's no validator for a given command/query, this is a no-op — most
// queries won't need one, only commands with real input to check.
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = validators.Select(v => v.Validate(context)).SelectMany(r => r.Errors).Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var error = Error.Validation("Validation.Failed", string.Join(" | ", failures.Select(f => f.ErrorMessage)));

        // TResponse is always Result or Result<T> here — but C# can't call a
        // static method "through" a generic type parameter polymorphically,
        // so this checks which concrete shape TResponse is and calls the
        // matching Failure directly, rather than guessing or letting an
        // exception happen deeper in the pipeline.
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var valueType = typeof(TResponse).GetGenericArguments()[0];
        var failureMethod = typeof(Result<>).MakeGenericType(valueType)
            .GetMethod(nameof(Result.Failure), [typeof(Error)])!;

        return (TResponse)failureMethod.Invoke(null, [error])!;
    }
}