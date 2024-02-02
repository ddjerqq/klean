using FluentValidation;
using FluentValidation.Internal;
using MediatR;

namespace Application.Common.Behaviours;

internal sealed class RequestValidationBehaviour<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var tasks = validators.Select(v =>
            v.ValidateAsync(request, opt => opt.IncludeAllRuleSets(), ct));

        var validationResults = await Task.WhenAll(tasks);

        if (validationResults.All(x => x.IsValid))
            return await next();

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        throw new ValidationException(failures);
    }
}