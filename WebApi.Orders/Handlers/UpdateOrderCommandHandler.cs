using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class UpdateOrderCommandHandler(AppDbContext appDbContext) : IConsumer<UpdateOrderCommand>
    {
        public async Task Consume(ConsumeContext<UpdateOrderCommand> context)
        {
            Order? order = appDbContext.Orders.FirstOrDefault(o => o.Id == context.Message.OrderId);

            order.Status = context.Message.Status;
            appDbContext.Update(order);
            await appDbContext.SaveChangesAsync();
        }
    }
}
