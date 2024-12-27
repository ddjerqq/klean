using Domain.Events;
using MediatR;

namespace Application.Cqrs.Users.Events;

internal sealed class UserLoggedInFromNewDeviceEventHandler : INotificationHandler<UserLoggedInFromNewDevice>
{
    public Task Handle(UserLoggedInFromNewDevice notification, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}