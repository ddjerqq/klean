using System.ComponentModel;
using Domain.Events;
using MediatR;

namespace Application.Inventory.Events;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class ItemReceivedEventHandler : INotificationHandler<ItemReceivedEvent>
{
    public Task Handle(ItemReceivedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Email sending is not implemented yet lol");
    }
}