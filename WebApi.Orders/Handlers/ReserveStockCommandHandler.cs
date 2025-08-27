using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ReserveStockCommandHandler(IPublishEndpoint publisher) : IConsumer<ReserveStockCommand>
    {
        public async Task Consume(ConsumeContext<ReserveStockCommand> context)
        {
            await publisher.Publish(new InventoryCheckPerformedEvent
            {
                OrderId = context.Message.OrderId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                CheckedAt = DateTime.UtcNow,
            }, context.CancellationToken);
        }
    }
}
