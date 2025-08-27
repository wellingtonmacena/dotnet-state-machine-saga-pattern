using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class ShipmentDispatchedEventListener(AppDbContext appDbContext) : IConsumer<ShipmentDispatchedEvent>
    {
        public async Task Consume(ConsumeContext<ShipmentDispatchedEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.Shipped;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
