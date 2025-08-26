using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Inventory.Consumer
{
    public class CheckProductsAvailableEventReceivedConsumer(IPublishEndpoint publisher) : IConsumer<CheckProductsAvailableEventReceived>
    {
        public Task Consume(ConsumeContext<CheckProductsAvailableEventReceived> context)
        {
            if(new Random().NextDouble() > 0.5)
            {
                return publisher.Publish(new ProductsAvailableChecked
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow
                });
            }
            else
            {
                return publisher.Publish(new ProductsUnavailableChecked
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow
                });
            }
        }
    }
}
