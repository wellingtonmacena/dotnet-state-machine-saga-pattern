using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class InventoryOutOfStockEventListener(AppDbContext appDbContext) : IConsumer<InventoryOutOfStockEvent>
    {
        public async Task Consume(ConsumeContext<InventoryOutOfStockEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockUnavailable;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
