using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Payments.Models;

namespace WebApi.Payments.Handlers
{
    public class PaymentInitiatedEventHandler(AppDbContext appDbContext) : IConsumer<PaymentInitiatedEvent>
    {
        public async Task Consume(ConsumeContext<PaymentInitiatedEvent> context)
        {
            double milliseconds = new Random().NextDouble();
            bool isSuccessful = milliseconds <= 0.5;

            Payment payment = new()
            {
                OrderId = context.Message.OrderId,
                Amount = context.Message.Amount,
                PaymentMethod = context.Message.PaymentMethod,
                PaymentDate = context.Message.PaidAt,
                Status = isSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed,
            };
            appDbContext.Payments.Add(payment);
            await appDbContext.SaveChangesAsync();

            var seconds = milliseconds * 20;
            Thread.Sleep(TimeSpan.FromSeconds(seconds));

            if (isSuccessful)
            {
                PaymentSucceededEvent paymentSuccessfulProcessed = new()
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
                PaymentFailedEvent paymentFailedProcessed = new()
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
