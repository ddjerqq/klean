using Domain.Events;
using MediatR;

namespace Application.Cqrs.Users.Events;

internal sealed class UserResetPasswordHandler : INotificationHandler<UserResetPassword>
{
    public Task Handle(UserResetPassword notification, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}