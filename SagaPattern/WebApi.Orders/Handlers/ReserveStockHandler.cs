using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ReserveStockHandler(IPublishEndpoint publisher) : IConsumer<ReserveStock>
    {
        public async Task Consume(ConsumeContext<ReserveStock> context)
        {
           
            await publisher.Publish(new CheckProductsAvailableEventReceived
            {
                OrderId = context.Message.OrderId,
               CheckedAt = DateTime.UtcNow
            }, context.CancellationToken);

         
        }
    }
}
