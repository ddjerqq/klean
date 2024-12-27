// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

using System.Globalization;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Events;
using Generated;

namespace Domain.Aggregates;

[StrongId]
public sealed class User(UserId id) : AggregateRoot<UserId>(id)
{
    public const int MaxAccessFailedCount = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public required string PersonalId { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public CultureInfo CultureInfo { get; init; } = CultureInfo.InvariantCulture;
    public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;

    public bool EmailConfirmed { get; set; }

    public string PasswordHash { get; private set; } = null!;
    public string SecurityStamp { get; private set; } = Guid.NewGuid().ToString();
    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString();
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }

    public ICollection<UserClaim> Claims { get; init; } = [];
    public ICollection<UserLogin> Logins { get; init; } = [];
    public ICollection<UserRole> Roles { get; init; } = [];

    public void SetPassword(string newPassword, bool isInitial = false)
    {
        SecurityStamp = Guid.NewGuid().ToString();
        PasswordHash = BC.EnhancedHashPassword(newPassword);

        if (!isInitial)
            AddDomainEvent(new UserResetPassword(Id));
    }
}