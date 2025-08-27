using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class RefundPaymentHandler(AppDbContext appDbContext) : IConsumer<RefundPaymentCommand>
    {
        public async Task Consume(ConsumeContext<RefundPaymentCommand> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = EStatus.PaymentFailed;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();

            PaymentRefundedEvent paymentRefunded = new()
            {
                OrderId = context.Message.OrderId,
                FailedAt = DateTime.UtcNow,
                Reason = context.Message.Reason
            };


            await context.Publish(paymentRefunded);
        }
    }
}
