using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class PaymentFailedEventListener(AppDbContext appDbContext) : IConsumer<PaymentFailedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.PaymentFailed;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
