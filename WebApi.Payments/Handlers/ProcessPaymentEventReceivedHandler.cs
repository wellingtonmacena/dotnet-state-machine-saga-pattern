using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Payments.Models;

namespace WebApi.Payments.Handlers
{
    public class ProcessPaymentEventReceivedHandler(AppDbContext appDbContext) : IConsumer<ProcessPaymentEventReceived>
    {
        public async Task Consume(ConsumeContext<ProcessPaymentEventReceived> context)
        {
            var isSuccessful = new Random().NextDouble() >= 0.5;

            Payment payment = new()
            {
                OrderId = context.Message.OrderId,
                Amount = context.Message.Amount,
                Method = context.Message.PaymentMethod,
                PaymentDate = context.Message.PaidAt,
                Status = isSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed,
            };
            appDbContext.Payments.Add(payment);
            await appDbContext.SaveChangesAsync();

            if (isSuccessful)
            {
                PaymentSuccessfulProcessed paymentSuccessfulProcessed = new()
                {
                    OrderId = context.Message.OrderId,
                    PaidAt = context.Message.PaidAt,
                    PaymentMethod = context.Message.PaymentMethod,
                    Amount = context.Message.Amount,
                };
                await context.Publish(paymentSuccessfulProcessed);
            }
            else
            {
                PaymentFailedProcessed paymentFailedProcessed = new()
                {
                    OrderId = context.Message.OrderId,
                    Reason = "Payment processing failed due to insufficient funds.",
                    PaymentMethod = context.Message.PaymentMethod,
                    FailedAt = DateTime.UtcNow,
                };
                await context.Publish(paymentFailedProcessed);
            }
        }
    }
}
