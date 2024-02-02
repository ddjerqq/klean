using System.ComponentModel;
using Domain.Events;
using MediatR;

namespace Application.Auth.Handlers;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class AuthEventsHandler
    : INotificationHandler<UserCreatedEvent>,
        INotificationHandler<LoginEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken ct)
    {
        throw new NotImplementedException("Email sending is not implemented.");
    }

    public Task Handle(LoginEvent notification, CancellationToken ct)
    {
        throw new NotImplementedException("Email sending is not implemented.");
    }
}