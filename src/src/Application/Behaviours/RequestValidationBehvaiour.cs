using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Serilog;
using ValidationException = FluentValidation.ValidationException;
using DataAnnotationsValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using FluentValidationResult = FluentValidation.Results.ValidationResult;

namespace Application.Behaviours;

internal sealed class RequestValidationBehaviour<TRequest, TResponse>(IServiceProvider serviceProvider, IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var context = new ValidationContext(request, null, null);
        var results = new List<DataAnnotationsValidationResult>();
        Validator.TryValidateObject(request, context, results, true);

        var tasks = validators.Select(v =>
            v.ValidateAsync(request, opt => opt.IncludeAllRuleSets(), ct));

        var fluentValidationResults = await Task.WhenAll(tasks);
        var dataAnnotationValidationResults = DataAnnotationValidate(request);
        var validationResults = fluentValidationResults.Concat(dataAnnotationValidationResults).ToList();

        if (validationResults.Any(x => !x.IsValid))
        {
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            Log.Logger.Warning("Validation failed for request {RequestName} {Request} with errors {Errors}", request.GetType().Name, request, string.Join(';', failures));

            throw new ValidationException(failures);
        }

        return await next();
    }

    private IEnumerable<FluentValidationResult> DataAnnotationValidate(TRequest request)
    {
        var context = new ValidationContext(request, serviceProvider, null);
        var results = new List<DataAnnotationsValidationResult>();

        Validator.TryValidateObject(request, context, results, true);

        var failures =
            from result in results
            let memberName = result.MemberNames.First()
            let errorMessage = result.ErrorMessage
            select new ValidationFailure(memberName, errorMessage);

        return failures.Select(failure => new FluentValidationResult([failure]));
    }
}