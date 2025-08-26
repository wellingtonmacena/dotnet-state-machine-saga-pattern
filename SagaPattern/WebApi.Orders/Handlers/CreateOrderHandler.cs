using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class CreateOrderHandler(AppDbContext appDbContext) : IConsumer<CreateOrder>
    {
        public async Task Consume(ConsumeContext<CreateOrder> context)
        {
            Order order = new()
            {
                CartId = context.Message.CartId,
                CustomerId = context.Message.CustomerId,
                TotalPrice = context.Message.TotalPrice,
            };

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Order> createdOrder = await appDbContext.Orders.AddAsync(order);
            await appDbContext.SaveChangesAsync();

            await context.Publish(new OrderCreated
            {
                OrderId = createdOrder.Entity.Id,
                CreatedAt = createdOrder.Entity.CreatedAt
            });
        }
    }
}
