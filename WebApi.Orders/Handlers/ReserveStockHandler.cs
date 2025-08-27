using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ReserveStockHandler(IPublishEndpoint publisher, AppDbContext appDbContext) : IConsumer<ReserveStockCommand>
    {
        public async Task Consume(ConsumeContext<ReserveStockCommand> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockReserved;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();

            await publisher.Publish(new InventoryCheckPerformedEvent
            {
                OrderId = context.Message.OrderId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                CheckedAt = DateTime.UtcNow,
            }, context.CancellationToken);

        }
    }
}
