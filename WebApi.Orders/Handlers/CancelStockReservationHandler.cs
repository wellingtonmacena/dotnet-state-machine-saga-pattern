using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class CancelStockReservationHandler(AppDbContext appDbContext) : IConsumer<CancelStockReservationCommand>
    {
        public async Task Consume(ConsumeContext<CancelStockReservationCommand> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockUnavailable;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
