using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours;

internal sealed class RequestValidationBehaviour
    <TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task
            .WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (validationResults.All(x => x.IsValid))
        {
            return await next();
        }

        throw new ValidationException(failures);
    }
}