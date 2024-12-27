using Domain.Events;
using MediatR;

namespace Application.Cqrs.Users.Events;

internal sealed class UserLockedOutEventHandler : INotificationHandler<UserLockedOut>
{
    public Task Handle(UserLockedOut notification, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}