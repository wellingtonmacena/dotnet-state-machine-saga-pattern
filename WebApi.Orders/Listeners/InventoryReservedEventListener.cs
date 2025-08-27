using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class InventoryReservedEventListener(AppDbContext appDbContext) : IConsumer<InventoryReservedEvent>
    {
        public async Task Consume(ConsumeContext<InventoryReservedEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockReserved;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
