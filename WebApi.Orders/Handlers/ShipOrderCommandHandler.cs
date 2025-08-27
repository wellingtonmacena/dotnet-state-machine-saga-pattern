using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ShipOrderCommandHandler(AppDbContext appDbContext) : IConsumer<ShipOrderCommand>
    {
        public async Task Consume(ConsumeContext<ShipOrderCommand> context)
        {
            ShipmentCreatedEvent shipmentOrderCreatedEvent = new()
            {
                OrderId = context.Message.OrderId,
                Address = context.Message.Address,
                ShippedAt = DateTime.UtcNow,
            };

            await context.Publish(shipmentOrderCreatedEvent);
        }
    }
}
