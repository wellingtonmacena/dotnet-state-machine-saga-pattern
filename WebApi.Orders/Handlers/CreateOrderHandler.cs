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
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                TotalPrice = context.Message.TotalPrice,
                Status = EStatus.Created
            };

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Order> createdOrder = await appDbContext.Orders.AddAsync(order);
            await appDbContext.SaveChangesAsync();

            await context.Publish(new OrderCreated
            {
                OrderId = createdOrder.Entity.Id,
                ProductId = createdOrder.Entity.ProductId,
                Quantity = createdOrder.Entity.Quantity,
                CreatedAt = createdOrder.Entity.CreatedAt
            });
        }
    }
}
