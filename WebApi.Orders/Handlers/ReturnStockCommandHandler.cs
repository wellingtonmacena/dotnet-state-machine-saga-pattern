using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ReturnStockCommandHandler(AppDbContext appDbContext) : IConsumer<ReturnStockCommand>
    {
        public async Task Consume(ConsumeContext<ReturnStockCommand> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.PaymentFailed;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();

            InventoryReleasedEvent inventoryReleasedEvent = new InventoryReleasedEvent() {
                OrderId = order.Id,
                ProductId = order.ProductId,
                Quantity = order.Quantity,
                ReleasedAt = DateTime.UtcNow
            };

            await context.Publish(inventoryReleasedEvent);
        }
    }
}
