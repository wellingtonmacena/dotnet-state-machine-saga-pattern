using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class ShipmentDeliveredEventListener(AppDbContext appDbContext) : IConsumer<ShipmentDeliveredEvent>
    {
        public async Task Consume(ConsumeContext<ShipmentDeliveredEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.Delivered;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
