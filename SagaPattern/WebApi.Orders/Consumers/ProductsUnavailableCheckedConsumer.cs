using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Consumers
{
    public class ProductsUnavailableCheckedConsumer(IPublishEndpoint publisher, IBus bus) : IConsumer<ProductsUnavailableChecked>
    {
        public async Task Consume(ConsumeContext<ProductsUnavailableChecked> context)
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
