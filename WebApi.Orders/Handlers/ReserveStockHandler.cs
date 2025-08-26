using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ReserveStockHandler(IPublishEndpoint publisher, AppDbContext appDbContext) : IConsumer<ReserveStock>
    {
        public async Task Consume(ConsumeContext<ReserveStock> context)
        {
           
            await publisher.Publish(new CheckProductsAvailableEventReceived
            {
                OrderId = context.Message.OrderId,
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                CheckedAt = DateTime.UtcNow
            }, context.CancellationToken);

            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockReserved;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();

        }
    }
}
