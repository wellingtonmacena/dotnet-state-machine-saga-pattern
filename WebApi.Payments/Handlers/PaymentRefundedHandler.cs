using Library.MessagingContracts.Messages;
using MassTransit;

namespace WebApi.Payments.Handlers
{
    public class PaymentRefundedHandler(AppDbContext appDbContext) : IConsumer<PaymentRefunded>
    {
        public async Task Consume(ConsumeContext<PaymentRefunded> context)
        {
            var payment = appDbContext.Payments.FirstOrDefault(p => p.OrderId  == context.Message.OrderId);

            if (payment != null) 
            {
                payment.Status = PaymentStatus.Refunded;
                appDbContext.Payments.Update(payment);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
