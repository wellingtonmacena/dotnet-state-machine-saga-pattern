using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class CancelStockReservationHandler(AppDbContext appDbContext) : IConsumer<CancelStockReservation>
    {
        public async Task Consume(ConsumeContext<CancelStockReservation> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.StockReservationFailed;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
