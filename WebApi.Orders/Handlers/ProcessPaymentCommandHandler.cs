using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class ProcessPaymentCommandHandler(AppDbContext appDbContext) : IConsumer<ProcessPaymentCommand>
    {
        public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
        {
            PaymentInitiatedEvent processPaymentEventReceived = new()
            {
                OrderId = context.Message.OrderId,
                PaidAt = DateTime.UtcNow,
                PaymentMethod = context.Message.PaymentMethod,
                Amount = context.Message.Amount,
            };

            await context.Publish(processPaymentEventReceived);
        }
    }
}
