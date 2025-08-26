using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Inventory.Consumer
{
    public class CheckProductsAvailableEventReceivedConsumer() : IConsumer<CheckProductsAvailableEventReceived>
    {
        public Task Consume(ConsumeContext<CheckProductsAvailableEventReceived> context)
        {
            if(new Random().NextDouble() > 0.5)
            {
                return context.Publish(new ProductsAvailableChecked
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow
                });
            }
            else
            {
                return context.Publish(new ProductsUnavailableChecked
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow
                });
            }
        }
    }
}
