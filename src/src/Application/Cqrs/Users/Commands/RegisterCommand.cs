using System.Globalization;
using Application.Services;
using Destructurama.Attributed;
using Domain.Aggregates;
using Domain.Events;
using EntityFrameworkCore.DataProtection.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.Users.Commands;

public sealed record RegisterCommand : IRequest<bool>
{
    [LogMasked]
    public string FullName { get; set; } = null!;

    [LogMasked]
    public string Email { get; set; } = null!;

    [LogMasked]
    public string PhoneNumber { get; set; } = null!;

    [LogMasked]
    public string PersonalId { get; set; } = null!;

    [LogMasked]
    public string Password { get; set; } = null!;

    [LogMasked]
    public TimeZoneInfo TimeZoneInfo { get; set; } = null!;

    [LogMasked]
    public CultureInfo CultureInfo { get; set; } = null!;

    public bool AgreeToTerms { get; set; }
}

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MinimumLength(5).MaximumLength(32);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(12).MaximumLength(256);
        RuleFor(x => x.PhoneNumber).NotEmpty().MinimumLength(10).MaximumLength(15);
        RuleFor(x => x.PersonalId).NotEmpty().Matches(@"(\d{11}|\d{3}\-\d{4}\-\d{3})").WithMessage("Must be 11 digit georgian ID or 3-4-3 digits american SSN");
        RuleFor(x => x.AgreeToTerms).Equal(true).WithMessage("You must agree to terms in order to register.");
    }
}

internal sealed class RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, ISender sender, IAppDbContext dbContext)
    : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken ct)
    {
        var usersWithEmail = await dbContext.Users.WherePdEquals(nameof(User.Email), request.Email.ToLowerInvariant()).CountAsync(ct);
        var usersWithPhoneId = await dbContext.Users.WherePdEquals(nameof(User.PhoneNumber), request.PhoneNumber).CountAsync(ct);
        var usersWithPersonalId = await dbContext.Users.WherePdEquals(nameof(User.PersonalId), request.PersonalId).CountAsync(ct);

        if (usersWithEmail + usersWithPhoneId + usersWithPersonalId > 0)
        {
            logger.LogWarning("User already registered email: {Email}, phone: {PhoneNumber}, personalId: {PersonalId}", request.Email, request.PhoneNumber, request.PersonalId);
            return false;
        }

        await using var transaction = await dbContext.BeginTransactionAsync(ct);

        var user = new User(UserId.New())
        {
            PersonalId = request.PersonalId,
            Username = request.FullName.ToLowerInvariant(),
            Email = request.Email.ToLowerInvariant(),
            PhoneNumber = request.PhoneNumber,
            CultureInfo = request.CultureInfo,
            TimeZone = request.TimeZoneInfo,
        };
        user.SetPassword(request.Password, true);

        user.AddDomainEvent(new UserRegistered(user.Id));

        await dbContext.Users.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        return true;
    }
}