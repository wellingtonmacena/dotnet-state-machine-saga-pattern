using Library.MessagingContracts.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Inventory.Handlers
{
    public class CheckProductsAvailableEventReceivedHandler(AppDbContext appDbContext) : IConsumer<InventoryCheckPerformedEvent>
    {
        public async Task Consume(ConsumeContext<InventoryCheckPerformedEvent> context)
        {
            bool isAvailable = appDbContext.StockItems.Any(si => si.ProductId == context.Message.ProductId && context.Message.Quantity <= si.Quantity);

            if (isAvailable)
            {
                Models.StockItem? stockItem = appDbContext.StockItems
                    .Include(si => si.Product)
                    .FirstOrDefault(p => p.ProductId == context.Message.ProductId);

                stockItem.Quantity -= context.Message.Quantity;
                appDbContext.StockItems.Update(stockItem);
                await appDbContext.SaveChangesAsync();

                await context.Publish(new InventoryReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow,
                    Quantity = context.Message.Quantity,
                    ProductId = context.Message.ProductId,
                    TotalPrice = stockItem.Product.Price * context.Message.Quantity

                });
            }
            else
            {

                await context.Publish(new InventoryOutOfStockEvent
                {
                    OrderId = context.Message.OrderId,
                    CheckedAt = DateTime.UtcNow,
                });


            }
        }
    }
}
