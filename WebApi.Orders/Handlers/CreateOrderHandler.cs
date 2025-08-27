using Library.MessagingContracts.Messages;
using MassTransit;
using WebApi.Orders.Dto;
using WebApi.Orders.Messages;

namespace WebApi.Orders.Handlers
{
    public class CreateOrderHandler(AppDbContext appDbContext, IHttpClientFactory _httpClientFactory) : IConsumer<CreateOrderCommand>
    {
        public async Task Consume(ConsumeContext<CreateOrderCommand> context)
        {
            HttpClient client = _httpClientFactory.CreateClient("InventoryClient");
            HttpResponseMessage response = await client.GetAsync($"/products/{context.Message.ProductId}");

            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception("Product not found");
            }

            ProductDto? product = await response.Content.ReadFromJsonAsync<ProductDto>();

            Order order = new()
            {
                ProductId = context.Message.ProductId,
                Quantity = context.Message.Quantity,
                Status = EStatus.Created,
                PaymentMethod = context.Message.PaymentMethod,
                TotalPrice = product.Price * context.Message.Quantity,
                Address = context.Message.Address,
            };

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Order> createdOrder = await appDbContext.Orders.AddAsync(order);
            await appDbContext.SaveChangesAsync();

            await context.Publish(new OrderCreatedEvent
            {
                OrderId = createdOrder.Entity.Id,
                ProductId = createdOrder.Entity.ProductId,
                TotalPrice = createdOrder.Entity.TotalPrice,
                Quantity = createdOrder.Entity.Quantity,
                CreatedAt = createdOrder.Entity.CreatedAt,
                PaymentMethod = createdOrder.Entity.PaymentMethod,
                Address = createdOrder.Entity.Address
            });
        }
    }
}
