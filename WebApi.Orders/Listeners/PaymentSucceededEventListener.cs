using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Orders.Listeners
{
    public class PaymentSucceededEventListener(AppDbContext appDbContext) : IConsumer<PaymentSucceededEvent>
    {
        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.PaymentProcessed;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
