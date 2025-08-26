using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Consumers
{
    public class ProductsAvailableCheckedConsumer(IPublishEndpoint publisher, IBus bus) : IConsumer<ProductsAvailableChecked>
    {
        public async Task Consume(ConsumeContext<ProductsAvailableChecked> context)
        {
            try
            {
                var eventq = new ProductsAvailableChecked() { CheckedAt = context.Message.CheckedAt, OrderId = context.Message.OrderId }
            ;
                await bus.Publish(eventq);
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }
    }
}
